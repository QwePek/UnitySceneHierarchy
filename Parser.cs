using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Program
{
    public class UnityParser
    {
        static void Main(string[] args)
        {
            //string projectPath = "G:\\Visual Studio Projects\\UnityProjectDumper";
            string projectPath = "G:\\Unity Projects\\Jumping Knight 3D";

            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTagMapping("tag:unity3d.com,2011:1", typeof(Gobjs))
                .WithTagMapping("tag:unity3d.com,2011:4", typeof(Trans))
                .WithTagMapping("tag:unity3d.com,2011:114", typeof(MonoBhv))
                .WithTagMapping("tag:unity3d.com,2011:1660057539", typeof(SCRoots));


            string pathToReferences = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "UnityClassIDReference.txt");
            try
            {
                // Otwieramy strumień do odczytu pliku
                using (StreamReader sr = new StreamReader(pathToReferences))
                {
                    // Odczytujemy plik linijka po linijce
                    string linia;
                    while ((linia = sr.ReadLine()) != null)
                    {
                        string ID = linia.Split('\t').First();
                        if (ID == "1" || ID == "4" || ID == "114")
                            continue;
                        deserializerBuilder.WithTagMapping("tag:unity3d.com,2011:" + ID, typeof(Skip));
                    }
                }
            }
            catch (Exception e)
            {
                // W przypadku błędu wyświetlamy komunikat
                Console.WriteLine("Wystąpił błąd: " + e.Message);
            }
            var deserializer = deserializerBuilder.Build();

            List<GameObject> allGameObjects = new List<GameObject>();
            List<Transform> allTransfroms = new List<Transform>();
            List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();
            SceneRoots? sceneRoots = null;

            string[] scenesPath = Directory.GetFiles(projectPath + "\\Assets", "*.unity", SearchOption.AllDirectories);

            foreach (string filePath in scenesPath)
            {
                List<GameObject> thisSceneGameObjects = new List<GameObject>();
                List<Transform> thisSceneTransforms = new List<Transform>();
                string sceneName = filePath.Split('\\').Last();

                try
                {
                    string wholeFile = File.ReadAllText(filePath/*projectPath + "\\SecondScene.unity"*/);
                    wholeFile = Regex.Replace(wholeFile, "([0-9]+ &[0-9]+) stripped\n", "$1\n"); //Deleting 'stripped' tag from unity
                                                                                                 //scene files (it allows me to open not only files attached in this exercise)
                    using (var sr = new StringReader(wholeFile))
                    {
                        var parser = new Parser(sr);
                        parser.Consume<StreamStart>();

                        while (!parser.Accept<StreamEnd>(out var _))
                        {
                            // Consume the stream start event "manually"
                            string anchor = "";
                            parser.Consume<DocumentStart>();

                            if (parser.Accept<MappingStart>(out var anch))
                                anchor = anch.Anchor.Value;

                            var doc = deserializer.Deserialize(parser);
                            parser.Consume<DocumentEnd>();

                            if (doc is Gobjs gameObject)
                            {
                                //Console.WriteLine("Dodano GameObject");
                                gameObject.gobj.anchor = anchor;
                                thisSceneGameObjects.Add(gameObject.gobj);
                                allGameObjects.Add(gameObject.gobj);
                            }
                            else if (doc is Trans transform)
                            {
                                //Console.WriteLine("Dodano Transform");
                                transform.trans.anchor = anchor;
                                if (transform.trans != null)
                                {
                                    thisSceneTransforms.Add(transform.trans);
                                    allTransfroms.Add(transform.trans);
                                }
                            }
                            else if (doc is SCRoots roots)
                            {
                                //Console.WriteLine("Dodano Scene Root");
                                roots.sceneRoots.anchor = anchor;
                                sceneRoots = roots.sceneRoots;
                            }
                            else if (doc is MonoBhv mono)
                            {
                                //Console.WriteLine("Dodano MonoBehaviour");
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

                foreach (GameObject g in thisSceneGameObjects)
                    g.parseTransforms(thisSceneTransforms);

                foreach (GameObject g in thisSceneGameObjects)
                    g.parseGameObjects(thisSceneGameObjects);

                //No scene root inside .unity file printing
                Console.WriteLine(sceneName + " hierarchy:");
                if (sceneRoots == null)
                {
                    List<GameObject> objs2 = new List<GameObject>();
                    foreach (GameObject g in thisSceneGameObjects)
                    {
                        if (g != null && g.transform != null && g.parent == null)
                        {
                            objs2.Add(g);
                        }
                    }

                    foreach (GameObject x in objs2)
                    {
                        x.print();
                    }
                }
                else
                    sceneRoots.printHierarchyTree(thisSceneGameObjects);

                Console.WriteLine("");
            }
            
            //Unused scripts finding
            //Finding every script inside project
            string[] scriptPaths = Directory.GetFiles(projectPath + "\\Assets", "*.cs.meta", SearchOption.AllDirectories);
            List<Script> allProjectScripts = new List<Script>();

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
                                allProjectScripts.Add(script);
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

            //Comparing every script inside project to used scripts
            List<Script> unusedScripts = new List<Script>();
            foreach (Script sc in allProjectScripts)
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
                    unusedScripts.Add(sc);
            }

            Console.WriteLine("Unused Scripts: ");
            foreach (Script sc in unusedScripts)
                Console.WriteLine(sc.path + ", " + sc.guid);
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

    public void print(int depth = 0)
    {
        for (int i = 0; i < depth; i++)
            Console.Write("--");
        Console.WriteLine(m_Name);

        if (transform == null)
            return;

        depth++;
        foreach (GameObject obj in children)
            obj.print(depth);
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

    public bool printHierarchyTree(List<GameObject> gameObjects)
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (FileID x in m_Roots)
        {
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                GameObject? g = gameObjects[i].findObjIDInside(x.fileID);
                if (g != null)
                {
                    objs.Add(g);
                    break;
                }
            }
        }

        foreach (GameObject x in objs)
        {
            x.print();
        }
        
        return true;
    }
}

public class FileID
{
    public string fileID { get; set; }
}