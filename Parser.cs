using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
            var deserializerBuilder = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTagMapping("tag:unity3d.com,2011:1", typeof(Gobjs))
                .WithTagMapping("tag:unity3d.com,2011:4", typeof(Trans))
                .WithTagMapping("tag:unity3d.com,2011:1660057539", typeof(SCRoots));

            for (int i = 0; i < 2000; i++)
            {
                if (i == 1 || i == 4)
                    continue;

                deserializerBuilder.WithTagMapping("tag:unity3d.com,2011:" + i.ToString(), typeof(Skip));
            }
            var deserializer = deserializerBuilder.Build();

            List<GameObject> gameObjects = new List<GameObject>();
            List<Transform> transfroms = new List<Transform>();
            SceneRoots? sceneRoots = null;

            try
            {
                using (var sr = new StreamReader("G:\\Visual Studio Projects\\UnityProjectDumper\\SecondScene.unity"))
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
                            gameObject.gobj.anchor = anchor;
                            gameObjects.Add(gameObject.gobj);
                        }
                        else if (doc is Trans transform)
                        {
                            transform.trans.anchor = anchor;
                            if(transform.trans != null)
                                transfroms.Add(transform.trans);
                        }
                        else if (doc is SCRoots roots)
                        {
                            roots.sceneRoots.anchor = anchor;
                            sceneRoots = roots.sceneRoots;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
            
            foreach (GameObject g in gameObjects)
                g.parseTransforms(transfroms);
            
            foreach (GameObject g in gameObjects)
                g.parseGameObjects(gameObjects);

            //if (sceneRoots == null)
            //{
            //    Console.WriteLine("Couldn't find scene roots inside .unity file... Aborting!");
            //    return;
            //}

            //Console.WriteLine("---GAME OBJECTS---");
            //foreach (GameObject g in gameObjects)
            //{
            //    Console.WriteLine(g.m_Name);
            //    Console.WriteLine("Parent: " + (g.transform.parentConn == null ? "null" : g.transform.parentConn.m_Name));
            //    if (g.transform.children.Count != 0)
            //    {
            //        Console.WriteLine("Children:");
            //        foreach (GameObject objs in g.transform.children)
            //        {
            //            Console.WriteLine("\t - " + objs.m_Name);
            //        }
            //    }
            //    Console.WriteLine("");
            //}

            //Console.WriteLine("\n---TRANSFORMS---");
            //foreach (Transform g in transfroms)
            //{
            //    Console.WriteLine(g.ToString());
            //}

            //Print withouth sceneRoots
            //List<GameObject> objs2 = new List<GameObject>();
            //foreach(GameObject g in gameObjects)
            //{
            //    if (g != null && g.transform != null && g.transform.parentConn == null)
            //    {
            //        objs2.Add(g);
            //    }
            //}

            //foreach (GameObject x in objs2)
            //{
            //    x.print();
            //}

            Console.WriteLine("\nROOTS:");
            Console.Write(sceneRoots.ToString());

            Console.WriteLine("\nHIREARCHY:");
            sceneRoots.printHierarchyTree(gameObjects);
        }
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