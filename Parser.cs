using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using UnityProjectDumper;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Program
{
    public class UnityParser
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowHelp();
                return;
            }

            string projectPath = args[0];
            string saveFolderPath = args[1];
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine("Could not find folder with path \'" + projectPath + "\'");
                return;
            }

            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTagMapping("tag:unity3d.com,2011:1", typeof(Gobjs))
                .WithTagMapping("tag:unity3d.com,2011:4", typeof(Trans))
                .WithTagMapping("tag:unity3d.com,2011:114", typeof(MonoBhv))
                .WithTagMapping("tag:unity3d.com,2011:1660057539", typeof(SCRoots));

            foreach (var idRef in IDReferences.list)
            {
                string ID = idRef.Item1;
                if (ID == "1" || ID == "4" || ID == "114" || ID == "1660057539")
                    continue;
                deserializerBuilder.WithTagMapping("tag:unity3d.com,2011:" + ID, typeof(Skip));
            }
            var deserializer = deserializerBuilder.Build();

            BlockingCollection<GameObject> allGameObjects = new BlockingCollection<GameObject>();
            BlockingCollection<Transform> allTransfroms = new BlockingCollection<Transform>();
            BlockingCollection<MonoBehaviour> monoBehaviours = new BlockingCollection<MonoBehaviour>();

            string[] scenesPath = Directory.GetFiles(projectPath + "\\Assets", "*.unity", SearchOption.AllDirectories);
            List<Task> tasks = new List<Task>();
            object deserializationLock = new object();

            foreach (string filePath in scenesPath)
            {
                tasks.Add(Task.Run(() =>
                {
                    List<GameObject> thisSceneGameObjects = new List<GameObject>();
                    List<Transform> thisSceneTransforms = new List<Transform>();
                    SceneRoots? sceneRoots = null;
                    string sceneName = filePath.Split('\\').Last();

                    try
                    {
                        string wholeFile = File.ReadAllText(filePath);
                        wholeFile = Regex.Replace(wholeFile, "([0-9]+ &[0-9]+) stripped\n", "$1\n"); //Deleting 'stripped' tag from unity
                                                                                                     //scene files (it allows me to open not only files attached in this exercise)
                        using (var sr = new StringReader(wholeFile))
                        {
                            var parser = new Parser(sr);
                            parser.Consume<StreamStart>();

                            while (!parser.Accept<StreamEnd>(out var _))
                            {
                                string anchor = "";
                                parser.Consume<DocumentStart>();

                                if (parser.Accept<MappingStart>(out var anch))
                                    anchor = anch.Anchor.Value;

                                object? doc = null;
                                lock (deserializationLock)
                                {
                                    doc = deserializer.Deserialize(parser);
                                }
                                parser.Consume<DocumentEnd>();

                                if (doc is Gobjs gameObject)
                                {
                                    gameObject.gobj.anchor = anchor;
                                    thisSceneGameObjects.Add(gameObject.gobj);
                                }
                                else if (doc is Trans transform)
                                {
                                    transform.trans.anchor = anchor;
                                    if (transform.trans != null)
                                        thisSceneTransforms.Add(transform.trans);
                                }
                                else if (doc is SCRoots roots)
                                {
                                    roots.sceneRoots.anchor = anchor;
                                    sceneRoots = roots.sceneRoots;
                                }
                                else if (doc is MonoBhv mono)
                                {
                                    mono.monoBehaviour.anchor = anchor;
                                    monoBehaviours.Add(mono.monoBehaviour);
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("Continuing...");
                    }

                    //Parsing Transforms with GameObjects
                    foreach (GameObject g in thisSceneGameObjects)
                        g.parseTransforms(thisSceneTransforms);

                    foreach (GameObject g in thisSceneGameObjects)
                        g.parseGameObjects(thisSceneGameObjects);

                    //Create save directory if doesn't exist
                    CreateSavePathDirectory(saveFolderPath);

                    //Get valid game objects (which are not containing not valid Transforms)
                    List<GameObject> validGobj = GetValidGameObjects(sceneRoots, thisSceneGameObjects);

                    //Saving scene hierarchies to unity.dump files
                    if (SaveHierarchyToDump(saveFolderPath + "/" + sceneName + ".dump", validGobj))
                        Console.WriteLine("Successfully saved " + sceneName + " hierarchy to file!");

                    foreach (var go in thisSceneGameObjects)
                        allGameObjects.Add(go);

                    foreach (var tr in thisSceneTransforms)
                        allTransfroms.Add(tr);
                }));
            }

            Task t = Task.WhenAll(tasks);
            try
            {
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("StackTrace: " + e.ToString());
            }

            List<MonoBehaviour> mono = new List<MonoBehaviour>(monoBehaviours);
            
            //Parse all scripts inside project to Script class
            List<Script> projectScripts = ParseScriptsInsideProject(projectPath);

            //Get unused scripts inside project
            List<Script> unusedScripts = GetUnusedScripts(projectScripts, mono);

            if (SaveScriptsToDump(saveFolderPath + "/UnusedScripts.txt", unusedScripts))
                Console.WriteLine("Successfully saved unused scripts to file!");
        }

        static List<GameObject> GetValidGameObjects(SceneRoots? sceneRoots, List<GameObject> sceneGameObjects)
        {
            //If SceneRoots is present we print hierarchy via Scene Roots IDs
            List<GameObject> validGameObjects = new List<GameObject>();
            if (sceneRoots != null)
            {
                foreach (FileID x in sceneRoots.m_Roots)
                {
                    foreach (GameObject g in sceneGameObjects)
                    {
                        GameObject? gobj = g.findObjIDInside(x.fileID);
                        if (gobj != null)
                        {
                            validGameObjects.Add(gobj);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (GameObject g in sceneGameObjects)
                    if (g != null && g.m_IsActive == 1 && g.transform != null && g.parent == null)
                        validGameObjects.Add(g);
            }

            return validGameObjects;
        }

        static bool SaveHierarchyToDump(string savePath, List<GameObject> validGameObjects)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(savePath);
                foreach (GameObject x in validGameObjects)
                    x.writeToFile(writer);

                writer.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        static List<Script> ParseScriptsInsideProject(string projectPath)
        {
            //Finding every script inside project
            string[] scriptPaths = Directory.GetFiles(projectPath + "\\Assets", "*.cs.meta", SearchOption.AllDirectories);
            List<Script> projectScripts = new List<Script>();
            foreach (string str in scriptPaths)
            {
                var scriptDeserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                try
                {
                    using (var sr = new StreamReader(str))
                    {
                        var parser = new Parser(sr);
                        parser.Consume<StreamStart>();

                        while (!parser.Accept<StreamEnd>(out var _))
                        {
                            // Consume the stream start event "manually"
                            var script = scriptDeserializer.Deserialize<Script>(parser);
                            if (script != null)
                            {
                                string relativePath = str.Remove(0, projectPath.Length + 1); //+1 because of '\' sign
                                script.path = relativePath.Remove(relativePath.Length - 5, 5); //Deleting .meta
                                projectScripts.Add(script);
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine("Continuing...");
                }
            }

            return projectScripts;
        }

        static List<Script> GetUnusedScripts(List<Script> allScripts, List<MonoBehaviour> monoBehaviours)
        {
            //Compare all project scripts with MonoBehaviours inside scenes
            List<Script> unused = new List<Script>();
            foreach (Script sc in allScripts)
            {
                bool found = false;
                foreach (MonoBehaviour bhv in monoBehaviours)
                {
                    if (sc.guid == bhv.m_Script.guid)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    unused.Add(sc);
            }

            return unused;
        }

        static bool SaveScriptsToDump(string savePath, List<Script> scripts)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(savePath))
                    foreach (Script sc in scripts)
                        writer.WriteLine(sc.path + ", " + sc.guid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: ./tool.exe unity_project_path output_folder_path");
            Console.WriteLine("");
            Console.WriteLine("Description:");
            Console.WriteLine("  This tool processes a Unity project located at the specified 'unity_project_path', reads");
            Console.WriteLine("  all hierarchies inside every .scene file, and finds unused scripts inside the");
            Console.WriteLine("  project. All of that is saved inside folder specified with 'output_folder_path'.");
        }

        static void CreateSavePathDirectory(string path)
        {
            if (Directory.Exists(path))
                return;
            
            Directory.CreateDirectory(path);
        }
    }
}

public class Script
{
    public string path { get; set; }

    public string fileID { get; set; }

    public string guid { get; set; }

    public int type { get; set; }

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Script:");
        strBuilder.AppendLine("\t path: " + path);
        strBuilder.AppendLine("\t fileID: " + fileID);
        strBuilder.AppendLine("\t guid: : " + guid);
        strBuilder.AppendLine("\t type: : " + type);

        return strBuilder.ToString();
    }
}

public class MonoBhv
{
    [YamlMember(Alias = "MonoBehaviour", ApplyNamingConventions = false)]
    public MonoBehaviour monoBehaviour { get; set; }

    public override string ToString()
    {
        return monoBehaviour.ToString();
    }
}

public class MonoBehaviour : Item
{
    [YamlMember(Alias = "m_Script", ApplyNamingConventions = false)]
    public Script m_Script { get; set; }

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("MonoBehaviour:");
        strBuilder.AppendLine("\t fileID: " + m_Script.fileID);
        strBuilder.AppendLine("\t guid: : " + m_Script.guid);
        strBuilder.AppendLine("\t type: : " + m_Script.type);

        return strBuilder.ToString();
    }
}

public class Skip
{

}

public abstract class Item
{
    public string anchor;
}

public class Gobjs
{
    [YamlMember(Alias = "GameObject", ApplyNamingConventions = false)]
    public GameObject gobj { get; set; }
    public override string ToString()
    {
        return gobj.ToString();
    }
}

public class Trans
{
    [YamlMember(Alias = "Transform", ApplyNamingConventions = false)]
    public Transform trans { get; set; }

    public override string ToString()
    {
        return trans.ToString();
    }
}

public class GameObject : Item
{
    [YamlMember(Alias = "m_ObjectHideFlags", ApplyNamingConventions = false)]
    public int m_ObjectHideFlags { get; set; }

    [YamlMember(Alias = "m_Component", ApplyNamingConventions = false)]
    public ComponentCl[] m_Component { get; set; }

    [YamlMember(Alias = "m_Name", ApplyNamingConventions = false)]
    public string m_Name { get; set; }

    [YamlMember(Alias = "m_TagString", ApplyNamingConventions = false)]
    public string m_TagString { get; set; }

    [YamlMember(Alias = "m_IsActive", ApplyNamingConventions = false)]
    public short m_IsActive { get; set; }

    public Transform transform = null;
    public GameObject parent = null;
    public List<GameObject> children = new List<GameObject>();

    public bool parseTransforms(List<Transform> transformList)
    {
        foreach (Transform t in transformList)
        {
            if (t.m_GameObject == null)
                continue;

            if (t.m_GameObject.fileID == anchor)
            {
                transform = t;
                transform.gameObjectConn = this;
                break;
            }
        }

        return false;
    }
    public bool parseGameObjects(List<GameObject> gobjsList)
    {
        if (transform == null)
            return false;

        //Splitted cause I want to add children in the same order as transform.m_Children
        //Children parsing
        if (transform.m_Children != null)
        {
            foreach (FileID f in transform.m_Children)
            {
                foreach (GameObject g in gobjsList)
                {
                    if (g == null)
                        continue;
                    if (g.transform == null)
                        continue;

                    if (g.transform.anchor == f.fileID)
                    {
                        children.Add(g);
                        break;
                    }
                }
            }
        }

        //Parent parsing
        foreach (GameObject g in gobjsList)
        {
            if (g == null)
                continue;
            if (g.transform == null)
                continue;

            if (transform.m_Father != null && transform.m_Father.fileID == g.transform.anchor)
            {
                parent = g;
                return true;
            }
        }

        return false;
    }
    public GameObject? findObjIDInside(string ID)
    {
        if (anchor == ID)
            return this;

        if (transform == null)
            return null;

        if (transform.anchor == ID)
            return this;

        foreach (GameObject child in children)
        {
            if (child == null)
                continue;

            GameObject? ret = child.findObjIDInside(ID);
            if (ret != null)
                return ret;
        }
        
        return null;
    }
    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("GAMEOBJECT: " + m_Name + "\t&" + anchor);
        if(parent != null)
            strBuilder.AppendLine("Parent: " + parent.m_Name + "\t&" + parent.anchor);

        if (children.Count != 0)
        {
            strBuilder.AppendLine("Children:");
            foreach (GameObject child in children)
            {
                strBuilder.AppendLine(child.ToString());
            }
        }
        if(transform != null)
            strBuilder.AppendLine(transform.ToString());
        return strBuilder.ToString();
    }
    public void writeToFile(StreamWriter writer, int depth = 0)
    {
        for (int i = 0; i < depth; i++)
            writer.Write("--");

        writer.WriteLine(m_Name);

        if (transform == null)
            return;

        depth++;
        foreach (GameObject obj in children)
            obj.writeToFile(writer, depth);
    }
}

public class ComponentCl
{
    [YamlMember(Alias = "component", ApplyNamingConventions = false)]
    public Component component { get; set; }
}

public class Component : Item
{
    public string fileID { get; set; }
}

public class Transform : Item
{
    [YamlMember(Alias = "m_Children", ApplyNamingConventions = false)]
    public FileID[] m_Children { get; set; }

    [YamlMember(Alias = "m_Father", ApplyNamingConventions = false)]
    public FileID m_Father { get; set; }

    [YamlMember(Alias = "m_GameObject", ApplyNamingConventions = false)]
    public FileID m_GameObject { get; set; }

    public GameObject gameObjectConn = null;

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Transform:\t&" + anchor);
        if (gameObjectConn != null)
            strBuilder.AppendLine("Game object connected: " + gameObjectConn.m_Name);
        return strBuilder.ToString();
    }
}

public class SCRoots
{
    [YamlMember(Alias = "SceneRoots", ApplyNamingConventions = false)]
    public SceneRoots sceneRoots { get; set; }

    public override string ToString()
    {
        return sceneRoots.ToString();
    }
}

public class SceneRoots : Item
{
    [YamlMember(Alias = "m_ObjectHideFlags", ApplyNamingConventions = false)]
    public int m_ObjectHideFlags { get; set; }
    [YamlMember(Alias = "m_Roots", ApplyNamingConventions = false)]
    public FileID[] m_Roots { get; set; }

    public override string ToString()
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine("Scene Roots &" + anchor);
        int i = 1;
        foreach (FileID c in m_Roots)
        {
            strBuilder.AppendLine(i + ": " + c.fileID);
            i++;
        }
        return strBuilder.ToString();
    }
}

public class FileID
{
    public string fileID { get; set; }
}