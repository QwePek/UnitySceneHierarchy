//using System;
//using System.ComponentModel;
//using System.IO;
//using System.Net.Security;
//using System.Text;
//using static SceneRoots;
//using static Transform;

//namespace Program
//{
//    class UnityDumper
//    {
//        static void Main(string[] args)
//        {
//            SceneRoots roots = new SceneRoots();
//            List<GameObject> gameObjects = new List<GameObject>();
//            List<Transform> transforms = new List<Transform>();

//            try
//            {
//                using (var sr = new StreamReader("G:\\Visual Studio Projects\\UnityProjectDumper\\SecondScene.unity"))
//                {
//                    bool writingGOBJ = false;
//                    bool readingRoots = false;
//                    bool readingTransform = false;
//                    int gobjID = -1;

//                    while (!sr.EndOfStream)
//                    {
//                        string? str = sr.ReadLine();
//                        if (str != null)
//                        {
//                            string first3chars = str.Substring(0, 3);
//                            if (str == "GameObject:")
//                            {
//                                writingGOBJ = true;
//                                gameObjects.Add(new GameObject());
//                                gobjID++;
//                                continue;
//                            }
//                            else if (str == "SceneRoots:")
//                            {
//                                readingRoots = true;
//                                continue;
//                            }
//                            else if (str == "Transform:")
//                            {
//                                readingTransform = true;
//                                continue;
//                            }
//                            else if (str[0] != ' ' && first3chars != "---")
//                            {
//                                writingGOBJ = false;
//                                readingRoots = false;
//                                readingTransform = false;
//                            }

//                            if (writingGOBJ)
//                            {
//                                if (first3chars == "---")
//                                {
//                                    string[] id = str.TrimStart().Split('&');
//                                    gameObjects[gobjID].m_fileID = uint.Parse(id[id.Length - 1]);

//                                    continue;
//                                }

//                                string[] splitted = str.TrimStart().Split(':');
//                                string attrib = splitted[0];
//                                string value = splitted[1].TrimStart();

//                                switch (attrib)
//                                {
//                                    case "m_ObjectHideFlags":
//                                        gameObjects[gobjID].m_ObjectHideFlags = int.Parse(value);
//                                        break;
//                                    case "m_Layer":
//                                        gameObjects[gobjID].m_Layer = int.Parse(value);
//                                        break;
//                                    case "m_Name":
//                                        gameObjects[gobjID].m_Name = value;
//                                        break;
//                                    case "m_TagString":
//                                        gameObjects[gobjID].m_TagString = value;
//                                        break;
//                                    case "m_NavMeshLayer":
//                                        gameObjects[gobjID].m_NavMeshLayer = int.Parse(value);
//                                        break;
//                                    case "m_StaticEditorFlags":
//                                        gameObjects[gobjID].m_StaticEditorFlags = int.Parse(value);
//                                        break;
//                                    case "m_IsActive":
//                                        if (value == "1")
//                                            gameObjects[gobjID].m_IsActive = true;
//                                        else
//                                            gameObjects[gobjID].m_IsActive = false;
//                                        break;
//                                    case "m_Component":
//                                        string? componentID = sr.ReadLine().TrimStart();
//                                        bool firstIter = true;

//                                        while (!sr.EndOfStream)
//                                        {
//                                            if (!firstIter)
//                                                componentID = sr.ReadLine().TrimStart();

//                                            if (componentID != null && componentID[0] != '-')
//                                                break;

//                                            string[] tmp = componentID.Split(':');
//                                            string fileID = tmp[tmp.Length - 1].TrimStart();
//                                            fileID = fileID.Remove(fileID.Length - 1, 1);
//                                            gameObjects[gobjID].m_ComponentID.Add(uint.Parse(fileID));

//                                            firstIter = false;
//                                        }

//                                        break;
//                                }
//                            }
//                            else if (readingRoots)
//                            {
//                                string[] splitted = str.TrimStart().Split(':');
//                                string attrib = splitted[0];
//                                string value = splitted[1];

//                                switch (attrib)
//                                {
//                                    case "m_ObjectHideFlags":
//                                        roots.m_ObjectHideFlags = int.Parse(value);
//                                        break;
//                                    case "m_Roots":
//                                        string? rootID = sr.ReadLine().TrimStart();
//                                        bool firstIter = true;

//                                        while (!sr.EndOfStream && (rootID != null && rootID[0] == '-'))
//                                        {
//                                            if (!firstIter)
//                                                rootID = sr.ReadLine().TrimStart();

//                                            string[] tmp = rootID.Split(':');
//                                            string fileID = tmp[tmp.Length - 1].TrimStart();
//                                            fileID = fileID.Remove(fileID.Length - 1, 1);
//                                            roots.m_Roots.Add(new GOBJConnect(uint.Parse(fileID)));

//                                            firstIter = false;
//                                        }

//                                        break;
//                                }
//                            }
//                            else if (readingTransform)
//                            {
//                                Transform readTransform = new Transform();

//                                while (!sr.EndOfStream)
//                                {
//                                    str = sr.ReadLine();
//                                    if (str != null)
//                                    {
//                                        first3chars = str.Substring(0, 3);

//                                        if (first3chars == "---")
//                                        {
//                                            string[] id = str.TrimStart().Split('&');
//                                            gameObjects[gobjID].m_fileID = uint.Parse(id[id.Length - 1]);

//                                            continue;
//                                        }

//                                        if (str[0] != ' ')
//                                            break;

//                                        string[] splitted = str.TrimStart().Split(':');
//                                        string attrib = splitted[0];
//                                        string value = splitted[1];

//                                        switch (attrib)
//                                        {
//                                            //public GOBJConnect m_GameObject = 0;
//                                            //public List<GOBJConnect> m_Children = new List<GOBJConnect>();

//                                            case "m_ObjectHideFlags":
//                                                readTransform.m_ObjectHideFlags = int.Parse(value);
//                                                break;
//                                            case "m_Father":
//                                                string fileID = splitted[splitted.Length - 1].TrimStart();
//                                                fileID = fileID.Remove(fileID.Length - 1, 1);
//                                                readTransform.m_Father = uint.Parse(fileID);
//                                                break;
//                                        }
//                                    }
//                                }
//                                transforms.Add(readTransform);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (IOException e)
//            {
//                Console.WriteLine(e.ToString());
//            }

//            if (!roots.parseRootsWithGameObjects(gameObjects))
//            {
//                Console.WriteLine("Couldn't parse GameObjects with Roots!");
//                return;
//            }

//            foreach (GameObject g in gameObjects)
//            {
//                if (!g.parseWithTransforms(transforms))
//                {
//                    Console.WriteLine("Couldn't parse Transform with GameObject!");
//                    return;
//                }
//            }

//            foreach (GameObject g in gameObjects)
//                g.print();

//            roots.print();
//        }
//    }
//}

//class GameObject
//{
//    public int m_ObjectHideFlags = 0;
//    public int m_Layer = 0;
//    public string m_Name = "GameObject";
//    public string m_TagString = "Untagged";
//    public int m_NavMeshLayer = 0;
//    public int m_StaticEditorFlags = 0;
//    public bool m_IsActive = true;
//    public uint m_fileID = 0;
//    public List<uint> m_ComponentID = new List<uint>();
//    public Transform? transform = null;

//    public void print()
//    {
//        Console.WriteLine(m_Name + " | ID: " + m_fileID);
//        Console.WriteLine("  Tag: " + m_TagString);
//        Console.WriteLine("  isActive: " + m_IsActive);
//        Console.WriteLine("  Static Editor Flags: " + m_StaticEditorFlags);
//        Console.WriteLine("  Layer: " + m_Layer);
//        Console.WriteLine("  Nav Mesh Layer: " + m_NavMeshLayer);
//        Console.WriteLine("  Object Hide Flags: " + m_ObjectHideFlags);
//        Console.WriteLine("  Components:");
//        foreach (uint c in m_ComponentID)
//            Console.WriteLine("    ID: " + c);
//        Console.WriteLine("  Transform:");
//        Console.WriteLine("    FatherID:" + transform.m_Father);

//        Console.WriteLine("----------------------------");
//    }

//    public bool parseWithTransforms(List<Transform> transf)
//    {
//        foreach (Transform g in transf)
//        {
//            uint transfID = g.m_fileID;
//            if (transfID == m_fileID)
//            {
//                transform = g;
//                return true;
//            }
//            foreach(uint u in m_ComponentID)
//                if(transfID == u)
//                {
//                    transform = g;
//                    return true;
//                }
//        }

//        return false;
//    }
//}

//class GOBJConnect
//{
//    public GOBJConnect(uint ID) { m_fileID = ID; }
//    public uint m_fileID;
//    public GameObject? m_obj = null;
//}

//class SceneRoots
//{
//    public List<GOBJConnect> m_Roots = new List<GOBJConnect>();
//    public int m_ObjectHideFlags = 0;

//    public void print()
//    {
//        Console.WriteLine("Scene Roots");
//        Console.WriteLine("  Roots:");
//        foreach (GOBJConnect r in m_Roots)
//            Console.WriteLine("    ID: " + r.m_fileID + " | Name: " + r.m_obj.m_Name);
//        Console.WriteLine("  Object Hide Flags: " + m_ObjectHideFlags);
//        Console.WriteLine("----------------------------");
//    }

//    public bool parseRootsWithGameObjects(List<GameObject> gobjs)
//    {
//        foreach (GameObject g in gobjs)
//        {
//            bool found = false;
//            uint gobjID = g.m_fileID;

//            for (int i = 0; i < m_Roots.Count; i++)
//            {
//                if (m_Roots[i].m_fileID == gobjID)
//                {
//                    m_Roots[i].m_obj = g;
//                    found = true;
//                    break;
//                }
//            }

//            if (found)
//                continue;

//            foreach (uint u in g.m_ComponentID)
//            {
//                gobjID = u;
//                for (int i = 0; i < m_Roots.Count; i++)
//                    if (m_Roots[i].m_fileID == gobjID)
//                    {
//                        m_Roots[i].m_obj = g;
//                        break;
//                    }
//            }
//        }

//        //Check if all root IDs found
//        foreach (GOBJConnect r in m_Roots)
//            if (r.m_obj == null)
//                return false;

//        return true;
//    }
//}

//class Transform
//{
//    public int m_ObjectHideFlags = 0;
//    //public GOBJConnect m_GameObject = 0;
//    public List<GOBJConnect> m_Children = new List<GOBJConnect>();
//    public uint m_Father = 0;
//    public uint m_fileID = 0;
//}