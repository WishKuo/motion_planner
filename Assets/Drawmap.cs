using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//first loop: robot num
//second loop: polygon num (set initial goal)
//third loop: vertex

public class Drawmap : MonoBehaviour
{
    public struct Vertice
    {
        public float vx;
        public float vy;
    }
    public struct PolygonVertice
    {
        public Vertice[] ver;
    }
    public struct RobotVertice
    {
        public PolygonVertice[] polyver;
    }
    public struct ObstacleVertice
    {
        public PolygonVertice[] polyver;
    }
    public struct RobotFinalVertice
    {
        public PolygonVertice[] polyver;
    }
    public struct Config    //(x,y,angle)
    {
        public float px;
        public float py;
        public float angle;
    }
    public struct PotentialPoint    //(x,y,angle)
    {
        public float px;
        public float py;
        public float angle;
        public bool visited;
        public PotentialPointParent parent;
    }
    public struct PotentialPointParent    //(x,y,angle)
    {
        public float px;
        public float py;
        public float angle;
    }
    public struct Controll  //control point: (x,y)
    {
        public float conpx;
        public float conpy;
    }
    public struct Apaircontroll //control point
    {
        public Controll[] conp;
    }
    int robnum = 0;
    int robpolynum = 0;
    int robvernum = 0;
    int obsnum = 0;
    int obspolynum = 0;
    int obsvernum = 0;
    int[,] map1 = new int[128, 128];
    int[,] map2 = new int[128, 128];
    RobotVertice[] robver;
    RobotFinalVertice[] robfver;
    ObstacleVertice[] obsver;
    Config[] robinitial;
    Config[] final;
    Config[] obsinitial;
    Apaircontroll[] pairpoint;
    List<Config> run = new List<Config>();
    public void ReadRobot() // read robot.txt
    {
        try
        {
            string path = "/Users/wish/desktop/university/IMLab/motionPlanner/motionPlanner/Assets/Data/robot.txt";
            //print("Hi~~~ Is anybody there??? QAQ"); //debug
		    if (!File.Exists(path)) return;
		    StreamReader sr = File.OpenText(path);
		    string strLine = sr.ReadLine();
            
            while (strLine != null)
            {
                if (strLine[0] != '#')
                {
                    robnum = Convert.ToInt32(strLine);
                    pairpoint = new Apaircontroll[robnum];
                    robver = new RobotVertice[robnum];
                    robfver = new RobotFinalVertice[robnum];
                    robinitial = new Config[robnum];
                    final = new Config[robnum];
                    for (int i = 0; i < robnum; i++)    // robot num
                    {
                        GameObject robot = new GameObject("robot" + i); // new robot
                        GameObject Final = new GameObject("final" + i); // new robotfinal
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')
                        {
                            strLine = sr.ReadLine();
                        }
                        robpolynum = Convert.ToInt32(strLine);
                        robver[i].polyver = new PolygonVertice[robpolynum];
                        robfver[i].polyver = new PolygonVertice[robpolynum];
                        for (int j = 0; j < robpolynum; j++)   // robpolynum
                        {
                            GameObject poly = new GameObject("poly" + j); // new polygon
                            poly.transform.parent = robot.transform;    // put polygons to robot
                            strLine = sr.ReadLine();
                            while (strLine[0] == '#')
                            {
                                strLine = sr.ReadLine();
                            }
                            robvernum = Convert.ToInt32(strLine);    // robot vertex num
                            robver[i].polyver[j].ver = new Vertice[robvernum];
                            for (int k = 0; k < robvernum; k++)  // obvernum
                            {
                                strLine = sr.ReadLine();
                                while (strLine[0] == '#')
                                {
                                    strLine = sr.ReadLine();
                                }
                                int z = 0;
                                while (strLine[z] != ' ') 
                                {
                                    z++;
                                }
                                float vx = Convert.ToSingle(strLine.Substring(0, z));    // x
                                float vy = Convert.ToSingle(strLine.Substring(z + 1, strLine.Length - 1 - z));  // y
                                robver[i].polyver[j].ver[k].vx = vx;   //加入X座標的點到第i個機器人的第j個多邊形的第k個頂點
                                robver[i].polyver[j].ver[k].vy = vy;   //加入Y座標的點到第i個機器人的第j個多邊形的第k個頂點
                            }
                            DrawRobot(i, j, robvernum, poly, robver);   // draw!!!!! AHHHHHHHHHHH
                        }
                        // initialization
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')
                        {
                            strLine = sr.ReadLine();
                        }
                        int a = 0;
                        while (strLine[a] != ' ')
                        {
                            a++;
                        }
                        float ix = Convert.ToSingle(strLine.Substring(0, a));   // x
                        string temp = strLine.Substring(a + 1, strLine.Length - 1 - a);
                        int b = 0;
                        while (temp[b] != ' ')
                        {
                            b++;
                        }
                        float iy = Convert.ToSingle(temp.Substring(0, b));  // y
                        float ia = Convert.ToSingle(temp.Substring(b + 1, temp.Length - 1 - b));    // angle
                        // read initial coordinater
                        robinitial[i].px = ix;
                        robinitial[i].py = iy;
                        robinitial[i].angle = ia;
                        // read goal
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')
                        {
                            strLine = sr.ReadLine();
                        }
                        int c = 0;
                        while (strLine[c] != ' ')
                        {
                            c++;
                        }
                        float fx = Convert.ToSingle(strLine.Substring(0, c));   // x
                        string temp1 = strLine.Substring(c + 1, strLine.Length - 1 - c);
                        int d = 0;
                        while (temp1[d] != ' ')
                        {
                            d++;
                        }
                        float fy = Convert.ToSingle(temp1.Substring(0, d));  // y
                        float fa = Convert.ToSingle(temp1.Substring(d + 1, temp1.Length - 1 - d));   // angle
                        final[i].px = fx;  // x (goal)
                        final[i].py = fy;  // y (goal)
                        final[i].angle = fa;   // angle (goal)
                        // read controll point
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')
                        {
                            strLine = sr.ReadLine();
                        }
                        int connum = Convert.ToInt32(strLine);  // control point number
                        pairpoint[i].conp = new Controll[connum];
                        for (int k = 0; k < connum; k++)
                        {
                            strLine = sr.ReadLine();
                            while (strLine[0] == '#')
                            {
                                strLine = sr.ReadLine();
                            }
                            int e = 0;
                            while (strLine[e] != ' ') 
                            {
                                e++;
                            }
                            float con1 = Convert.ToSingle(strLine.Substring(0, e));                             // x
                            float con2 = Convert.ToSingle(strLine.Substring(e + 1, strLine.Length - 1 - e));    // y
                            pairpoint[i].conp[k].conpx = con1;
                            pairpoint[i].conp[k].conpy = con2;
                            GameObject controll = new GameObject("controll" + k);
                            controll.transform.parent = robot.transform;
                            controll.transform.position = new Vector3(con1, con2, 0);
                        }
                        // initialization
                        Rotation(robot, robinitial[i]);
                        Posistion(robot, robinitial[i]);
                        
                        // draw goal
                        for (int p = 0; p < robpolynum; p++)
                        {
                            GameObject poly = new GameObject("poly" + p);
                            poly.transform.parent = Final.transform;
                            robfver[i].polyver[p].ver = new Vertice[robvernum];
                            DrawFinal(i, p, robvernum, poly, robver);
                            /*for (int q = 0; q < robvernum; q++)
                            {
                                float x = 0, y = 0;
                                x = Convert.ToSingle(Mathf.Cos(fangle) * robver[i].polyver[p].ver[q].vx - Mathf.Sin(fangle) * robver[i].polyver[p].ver[q].vy + final[i].px);
                                y = Convert.ToSingle(Mathf.Sin(fangle) * robver[i].polyver[p].ver[q].vx + Mathf.Cos(fangle) * robver[i].polyver[p].ver[q].vy + final[i].py);
                                robfver[i].polyver[p].ver[q].vx = x;
                                robfver[i].polyver[p].ver[q].vy = y;
                            } */
                        }
                        Rotation(Final, final[i]);
                        Posistion(Final, final[i]);
                        // updateeeee
                        float angle = robinitial[i].angle * Mathf.Deg2Rad;
                        for (int p = 0; p < robpolynum; p++)
                        {
                            for (int q = 0; q < robvernum; q++)
                            {
                                float x = 0, y = 0;
                                x = Convert.ToSingle(Mathf.Cos(angle) * robver[i].polyver[p].ver[q].vx - Mathf.Sin(angle) * robver[i].polyver[p].ver[q].vy + robinitial[i].px);
                                y = Convert.ToSingle(Mathf.Sin(angle) * robver[i].polyver[p].ver[q].vx + Mathf.Cos(angle) * robver[i].polyver[p].ver[q].vy + robinitial[i].py);
                                robver[i].polyver[p].ver[q].vx = x;
                                robver[i].polyver[p].ver[q].vy = y;
                            }
                        }
                        
                    }
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }

    public void ReadObstacle()  // read obstacle.txt
    {
        try
        {
            string path = "/Users/wish/desktop/university/IMLab/motionPlanner/motionPlanner/Assets/Data/obstacle.txt"; 
		    if (!File.Exists(path)) return;
		    StreamReader sr = File.OpenText(path);
		    string strLine = sr.ReadLine();

            while (strLine != null)
            {
                if (strLine[0] != '#')
                {
                    obsnum = Convert.ToInt32(strLine);  // obs num
                    print("Start!!!");
                    obsver = new ObstacleVertice[obsnum];
                    obsinitial = new Config[obsnum];
                    for (int i = 0; i < obsnum; i++)     // obsnum
                    {
                        GameObject obstacle = new GameObject("obstacle" + i);   // new obs
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')   
                        {
                            strLine = sr.ReadLine();
                        }
                        obspolynum = Convert.ToInt32(strLine);  // obs ploy num
                        obsver[i].polyver = new PolygonVertice[obspolynum];
                        for (int j = 0; j < obspolynum; j++)   // obsploynum
                        {
                            GameObject poly = new GameObject("poly");   // new poly
                            poly.transform.parent = obstacle.transform; // put poly to obstacle
                            strLine = sr.ReadLine();
                            while (strLine[0] == '#')   
                            {
                                strLine = sr.ReadLine();
                            }
                            obsvernum = Convert.ToInt32(strLine);   // obs vertex
                            obsver[i].polyver[j].ver = new Vertice[obsvernum];
                            for (int k = 0; k < obsvernum; k++)  // obsvernum
                            {
                                strLine = sr.ReadLine();
                                while (strLine[0] == '#')   
                                {
                                    strLine = sr.ReadLine();
                                }
                                int z = 0;
                                while (strLine[z] != ' ')  
                                {
                                    z++;
                                }
                                float vx = Convert.ToSingle(strLine.Substring(0, z));   // x
                                float vy = Convert.ToSingle(strLine.Substring(z + 1, strLine.Length - 1 - z));  // y
                                obsver[i].polyver[j].ver[k].vx = vx;   //加入X座標的點到第i個障礙物的第j個多邊形的第k個頂點
                                obsver[i].polyver[j].ver[k].vy = vy;   //加入Y座標的點到第i個障礙物的第j個多邊形的第k個頂點
                            }
                            DrawObstacle(i, j, obsvernum, poly, obsver);    // draw!!!!!!!!!RRRRRR
                        }
                        // read initial 
                        strLine = sr.ReadLine();
                        while (strLine[0] == '#')   
                        {
                            strLine = sr.ReadLine();
                        }
                        int a = 0;
                        while (strLine[a] != ' ')   
                        {
                            a++;
                        }
                        float ix = Convert.ToSingle(strLine.Substring(0, a));
                        string temp = strLine.Substring(a + 1, strLine.Length - 1 - a);
                        int b = 0;
                        while (temp[b] != ' ')
                        {
                            b++;
                        }
                        float iy = Convert.ToSingle(temp.Substring(0, b));
                        float ia = Convert.ToSingle(temp.Substring(b + 1, temp.Length - 1 - b));
                        obsinitial[i].px = ix;
                        obsinitial[i].py = iy;
                        obsinitial[i].angle = ia;
                        // initialization
                        Rotation(obstacle, obsinitial[i]);
                        Posistion(obstacle, obsinitial[i]);
                        float angle = obsinitial[i].angle * Mathf.Deg2Rad;
                        for (int p = 0; p < obspolynum; p++)    // update vertex
                        {
                            for (int q = 0; q < obsvernum; q++)
                            {
                                float x = 0, y = 0;
                                x = Convert.ToSingle(Mathf.Cos(angle) * obsver[i].polyver[p].ver[q].vx - Mathf.Sin(angle) * obsver[i].polyver[p].ver[q].vy + obsinitial[i].px);
                                y = Convert.ToSingle(Mathf.Sin(angle) * obsver[i].polyver[p].ver[q].vx + Mathf.Cos(angle) * obsver[i].polyver[p].ver[q].vy + obsinitial[i].py);
                                obsver[i].polyver[p].ver[q].vx = x;
                                obsver[i].polyver[p].ver[q].vy = y;
                            }
                        }
                    }
                }

                strLine = sr.ReadLine();
            }
            sr.Close();
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }

    public void DrawRobot(int robnum, int polynum, int vnum, GameObject obj, RobotVertice[] robver)
    {
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Material defaultMat = new Material(Shader.Find("Custom/NewSurfaceShader"));

        mr.sharedMaterial = defaultMat;

        Vector3[] vertices = new Vector3[vnum];
        int[] triangles = new int[(vnum - 2) * 3];

        for (int i = 0; i < vnum; i++)
        {
            vertices[i].Set(robver[robnum].polyver[polynum].ver[i].vx, robver[robnum].polyver[polynum].ver[i].vy, 0);
        }
        mf.mesh.vertices = vertices;
        for (int i = 0; i < vnum - 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    triangles[i * 3 + j] = 0;
                }
                else
                {
                    triangles[i * 3 + j] = i + 3 - j;
                }
            }
        }

        mf.mesh.triangles = triangles;
        obj.AddComponent<Drag>();
        obj.AddComponent<PolygonCollider2D>();
        Vector2[] arVec2 = new Vector2[vnum];
        for(int i = 0; i < vnum; i++)
        {
            arVec2[i].Set(robver[robnum].polyver[polynum].ver[i].vx, robver[robnum].polyver[polynum].ver[i].vy);
        }
        obj.GetComponent<PolygonCollider2D>().SetPath(0, arVec2);
    }

    public void DrawFinal(int robnum, int polynum, int vnum, GameObject obj, RobotVertice[] robver)
    {
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();

        Material defaultMat = new Material(Shader.Find("Sprites/Diffuse"));

        mr.sharedMaterial = defaultMat;

        Vector3[] vertices = new Vector3[vnum];
        int[] triangles = new int[(vnum - 2) * 3];

        for (int i = 0; i < vnum; i++)
        {
            vertices[i].Set(robver[robnum].polyver[polynum].ver[i].vx, robver[robnum].polyver[polynum].ver[i].vy, 0);
        }
        mf.mesh.vertices = vertices;
        for (int i = 0; i < vnum - 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    triangles[i * 3 + j] = 0;
                }
                else
                {
                    triangles[i * 3 + j] = i + 3 - j;
                }
            }
        }

        mf.mesh.triangles = triangles;
        obj.AddComponent<Drag>();
        obj.AddComponent<PolygonCollider2D>();
        Vector2[] arVec2 = new Vector2[vnum];
        for (int i = 0; i < vnum; i++)
        {
            arVec2[i].Set(robver[robnum].polyver[polynum].ver[i].vx, robver[robnum].polyver[polynum].ver[i].vy);
        }
        obj.GetComponent<PolygonCollider2D>().SetPath(0, arVec2);
    }

    public void DrawObstacle(int obsnum, int polynum, int vnum, GameObject obj, ObstacleVertice[] obsver)
    {
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        Material defaultMat = new Material(Shader.Find("Sprites/Default"));

        mr.sharedMaterial = defaultMat;

        Vector3[] vertices = new Vector3[vnum];
        int[] triangles = new int[(vnum - 2) * 3];

        for (int i = 0; i < vnum; i++)
        {
            vertices[i].Set(obsver[obsnum].polyver[polynum].ver[i].vx, obsver[obsnum].polyver[polynum].ver[i].vy, 0);
        }
        mf.mesh.vertices = vertices;
        for (int i = 0; i < vnum - 2; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j == 0)
                {
                    triangles[i * 3 + j] = 0;
                }
                else
                {
                    triangles[i * 3 + j] = i + 3 - j;
                }
            }
        }

        mf.mesh.triangles = triangles;
        obj.AddComponent<Drag>();
        obj.AddComponent<PolygonCollider2D>();
        Vector2[] arVec2 = new Vector2[vnum];
        for (int i = 0; i < vnum; i++)
        {
            arVec2[i].Set(obsver[obsnum].polyver[polynum].ver[i].vx, obsver[obsnum].polyver[polynum].ver[i].vy);
        }
        obj.GetComponent<PolygonCollider2D>().SetPath(0, arVec2);
    }

    public void Rotation(GameObject obj, Config initial)
    {
        obj.transform.rotation = Quaternion.Euler(0, 0, initial.angle);
    }

    public void Posistion(GameObject obj, Config initial)
    {
        obj.transform.position = new Vector3(initial.px, initial.py, 0);

    }

    public void Destroy()
    {
        for(int i = 0; i < robnum; i++)
        {
            Destroy(GameObject.Find("robot" + i));
        }
        for (int i = 0; i < robnum; i++)
        {
            Destroy(GameObject.Find("final" + i));
        }
        for (int i = 0; i < obsnum; i++)
        {
            Destroy(GameObject.Find("obstacle" + i));
        }
        Destroy(GameObject.Find("field"));
    }

    public void Destroyobj()
    {
        for (int i = 0; i < robnum; i++)
        {
            GameObject obj = GameObject.Find("robot" + i);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -1f);
        }
        for (int i = 0; i < robnum; i++)
        {
            GameObject obj = GameObject.Find("final" + i);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -1f);
        }
        for (int i = 0; i < obsnum; i++)
        {
            GameObject obj = GameObject.Find("obstacle" + i);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -1f);
        }
        Destroy(GameObject.Find("field"));
    }
    public void PotentialFeild()        // 128*128 px  254:not yet travel 255:obs 0:goal 
    {
        float fx = GameObject.Find("final0").transform.position.x;
        float fy = GameObject.Find("final0").transform.position.y;
        float angle = GameObject.Find("final0").transform.eulerAngles.z * Mathf.Deg2Rad;
        for (int k = 0; k < 2; k++)
        {
            float x, y;
            x = Convert.ToSingle(Mathf.Cos(angle) * pairpoint[0].conp[k].conpx - Mathf.Sin(angle) * pairpoint[0].conp[k].conpy + fx);
            y = Convert.ToSingle(Mathf.Sin(angle) * pairpoint[0].conp[k].conpx + Mathf.Cos(angle) * pairpoint[0].conp[k].conpy + fy);
            pairpoint[0].conp[k].conpx = x;
            pairpoint[0].conp[k].conpy = y;
            DrawPotentialFeild(pairpoint[0].conp[k].conpx, pairpoint[0].conp[k].conpy = y, k);
        }
    }

    public void DrawPotentialFeild(float F_x,float F_y,int draw)    
    {
        float Pos_x = 0, Pos_y = 0;
        if (draw == 0)
        {
            Pos_x = -320;
            Pos_y = 160;
        }
        else
        {
            Pos_x = -320;
            Pos_y = -160;
        }
        GameObject canvasGO = new GameObject("field");
        RectTransform canvasRT = canvasGO.AddComponent<RectTransform>();
        Canvas canvasCV = canvasGO.AddComponent<Canvas>();
        canvasCV.renderMode = RenderMode.ScreenSpaceCamera;
        Vector3 pos = Camera.main.transform.position;
        pos += Camera.main.transform.forward * 10.0f;
        canvasCV.worldCamera = Camera.main;
        GameObject buttonGO = new GameObject();
        RectTransform buttonRT = buttonGO.AddComponent<RectTransform>();
        buttonRT.SetParent(canvasRT);
        buttonRT.sizeDelta = new Vector2(128.0f, 128.0f);
        buttonRT.localPosition = new Vector3(Pos_x, Pos_y, -1f);
        Button buttonBU = buttonGO.AddComponent<Button>();
        buttonBU.onClick.AddListener(() => { Debug.Log("button clicked"); });
        Image buttonIM = buttonGO.AddComponent<Image>();
        buttonIM.sprite = Resources.Load("buttonSprite", typeof(Sprite)) as Sprite;

        // Create a new texture and assign it to the renderer's material


        Texture2D texture = new Texture2D(128, 128);

        int[,] map = new int[128, 128];
        int M = 254;
        int N = 255;
        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                Vector2 vector2 = new Vector2(i, j);
                Vector2 vector0 = new Vector2(0, 0);
                RaycastHit2D hit = Physics2D.Raycast(vector2, vector0, 10, -1);
                if (hit.collider && hit.transform.gameObject.name == "poly")
                {
                    map[i, j] = N;
                    Color32 blackcolor = new Color32(0, 0, 0, 255);
                    texture.SetPixel(i, j, blackcolor);
                }  
                else
                {
                    map[i, j] = M;
                }
                    
            }
        }
        
        

        List<List<Vertice>> list = new List<List<Vertice>>();
        List<Vertice> sublist = new List<Vertice>();
        // where potential field starts
        int x = Convert.ToInt32(F_x);
        int y = Convert.ToInt32(F_y);
        map[x, y] = 0;
        Vertice point = new Vertice
        {
            vx = x,
            vy = y
        };
        Color32 firstcolor = new Color32(1, 0, 0, 0);
        texture.SetPixel(Convert.ToInt32(point.vx), Convert.ToInt32(point.vy), firstcolor);
        sublist.Add(point);
        list.Add(sublist);

        for (int k = 0; ; k++)  //for i=0,1,...,until L[i] is empty do
        {
            sublist = new List<Vertice>();
            bool havelist = false;
            for (int m = 0; m < list[k].Count; m++) //for every q in L[i] do
            {
                if (((list[k][m].vx + 1) >= 0) && ((list[k][m].vx + 1) < 128))
                {
                    if (map[Convert.ToInt32(list[k][m].vx + 1), Convert.ToInt32(list[k][m].vy)] == M)
                    {
                        map[Convert.ToInt32(list[k][m].vx + 1), Convert.ToInt32(list[k][m].vy)] = k + 1;
                        point = new Vertice
                        {
                            vx = list[k][m].vx + 1,
                            vy = list[k][m].vy
                        };
                        sublist.Add(point);
                        havelist = true;
                        Color32 color = new Color32(0, 0, 0, Convert.ToByte(k + 1));
                        texture.SetPixel(Convert.ToInt32(point.vx), Convert.ToInt32(point.vy), color);
                    }
                }
                if (((list[k][m].vy + 1) >= 0) && ((list[k][m].vy + 1) < 128))
                {
                    if (map[Convert.ToInt32(list[k][m].vx), Convert.ToInt32(list[k][m].vy + 1)] == M)
                    {
                        map[Convert.ToInt32(list[k][m].vx), Convert.ToInt32(list[k][m].vy + 1)] = k + 1;
                        point = new Vertice
                        {
                            vx = list[k][m].vx,
                            vy = list[k][m].vy + 1
                        };
                        sublist.Add(point);
                        havelist = true;
                        Color32 color = new Color32(0, 0, 0, Convert.ToByte(k + 1));
                        texture.SetPixel(Convert.ToInt32(point.vx), Convert.ToInt32(point.vy), color);
                    }
                }
                if (((list[k][m].vx - 1) >= 0) && ((list[k][m].vx - 1) < 128))
                {
                    if (map[Convert.ToInt32(list[k][m].vx - 1), Convert.ToInt32(list[k][m].vy)] == M)
                    {
                        map[Convert.ToInt32(list[k][m].vx - 1), Convert.ToInt32(list[k][m].vy)] = k + 1;
                        point = new Vertice
                        {
                            vx = list[k][m].vx - 1,
                            vy = list[k][m].vy
                        };
                        sublist.Add(point);
                        havelist = true;
                        Color32 color = new Color32(0, 0, 0, Convert.ToByte(k + 1));
                        texture.SetPixel(Convert.ToInt32(point.vx), Convert.ToInt32(point.vy), color);
                    }
                }
                if (((list[k][m].vy - 1) >= 0) && ((list[k][m].vy - 1) < 128))
                {
                    if (map[Convert.ToInt32(list[k][m].vx), Convert.ToInt32(list[k][m].vy - 1)] == M)
                    {
                        map[Convert.ToInt32(list[k][m].vx), Convert.ToInt32(list[k][m].vy - 1)] = k + 1;
                        point = new Vertice
                        {
                            vx = list[k][m].vx,
                            vy = list[k][m].vy - 1
                        };
                        sublist.Add(point);
                        havelist = true;
                        Color32 color = new Color32(0, 0, 0, Convert.ToByte(k + 1));
                        texture.SetPixel(Convert.ToInt32(point.vx), Convert.ToInt32(point.vy), color);
                    }
                }
            }
            if (havelist)
            {
                list.Add(sublist);
            }
            else
            {
                break;
            }
        }
        //Apply all SetPixel calls
        texture.Apply();
        buttonIM.material.mainTexture = texture;
        if (draw == 0)
        {
            map1 = map;
        }
        else
        {
            map2 = map;
        }
    }

    public void BFS()
    {
        int count = 0;
        List<PotentialPoint>[,,] T = new List<PotentialPoint>[128, 128, 360];

        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                for (int k = 0; k < 360; k++)
                {
                    T[i, j, k] = new List<PotentialPoint>();
                }
            }
        }

        PotentialPoint op;
        for (int i = 0; i < 128; i++)
        {
            for(int j = 0; j < 128; j++)
            {
                for(int k = 0; k < 360; k++)
                {
                    op = new PotentialPoint
                    {
                        px = i,
                        py = j,
                        angle = k,
                        visited = false,
                    };
                    T[i, j, k].Add(op);
                }
            }
        }
        bool success = false;
        GameObject ctrl1 = GameObject.Find("robot0/controll0");
        GameObject ctrl2 = GameObject.Find("robot0/controll1");
        List<PotentialPoint>[] open = new List<PotentialPoint>[255];
        for(int i = 0; i < 255; i++)
        {
            open[i] = new List<PotentialPoint>();
        }

        int ix = Convert.ToInt32(GameObject.Find("robot0").transform.position.x);
        int iy = Convert.ToInt32(GameObject.Find("robot0").transform.position.y);
        int ia = Convert.ToInt32(GameObject.Find("robot0").transform.eulerAngles.z);
        PotentialPoint initial = T[ix, iy, ia][0];
        initial.visited = true;
        T[ix, iy, ia][0] = initial;
        int c1x = Convert.ToInt32(ctrl1.transform.position.x);
        int c1y = Convert.ToInt32(ctrl1.transform.position.y);
        int c2x = Convert.ToInt32(ctrl2.transform.position.x);
        int c2y = Convert.ToInt32(ctrl2.transform.position.y);
        int p = (map1[c1x, c1y] + map2[c2x, c2y]) / 2;
        open[p].Insert(0, initial);
        GameObject temp = Instantiate(GameObject.Find("robot0"));
        temp.transform.name = "temprob";
        Destroy(GameObject.Find("temprob/poly0").GetComponent<PolygonCollider2D>());
        //Destroy(GameObject.Find("temprob/poly1").GetComponent<PolygonCollider2D>());
        int ti = 0, tj = 0, ta = 0;
        while (success == false)
        {
            bool haveopen = true;
            for (int i = 0; i < 255; i++)
            {
                if (open[i].Count != 0)     // get first-x
                {
                    //print(i);
                    int Ti = Convert.ToInt32(open[i][0].px);
                    int Tj = Convert.ToInt32(open[i][0].py);
                    int Tk = Convert.ToInt32(open[i][0].angle);
                    //print(T[Ti, Tj, Tk][0].px + "&" + T[Ti, Tj, Tk][0].py + "&" + T[Ti, Tj, Tk][0].angle);
                    open[i].RemoveAt(0);
                    count++;
                    if (Ti + 1 < 128) 
                    {
                        op = new PotentialPoint();
                        op = T[Ti + 1, Tj, Tk][0];
                        temp.transform.position = new Vector3(Ti + 1, Tj, 0);
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti + 1, Tj, Tk][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti + 1;
                                tj = Tj;
                                ta = Tk;
                            }
                        }
                    }
                    if (Tj + 1 < 128)
                    {
                        op = new PotentialPoint();
                        op = T[Ti, Tj + 1, Tk][0];
                        temp.transform.position = new Vector3(Ti, Tj+1, 0);
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti, Tj + 1, Tk][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti;
                                tj = Tj+1;
                                ta = Tk;
                            }
                        }
                    }
                    if (true)
                    {
                        int theta = Tk;
                        if (Tk + 4 >= 360)
                        {
                            theta = Tk - 356;
                            temp.transform.eulerAngles = new Vector3(0, 0, theta);
                        }
                        else
                        {
                            theta = Tk + 4;
                            temp.transform.eulerAngles = new Vector3(0, 0, theta);
                        }
                        op = new PotentialPoint();
                        op = T[Ti, Tj, theta][0];
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti, Tj, theta][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti;
                                tj = Tj;
                                ta = theta;
                            }
                        }
                    }
                    if (Ti - 1 >= 0)
                    {
                        op = new PotentialPoint();
                        op = T[Ti - 1, Tj, Tk][0];
                        temp.transform.position = new Vector3(Ti - 1, Tj, 0);
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti - 1, Tj, Tk][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti-1;
                                tj = Tj;
                                ta = Tk;
                            }
                        }
                    }
                    if (Tj - 1 >= 0)
                    {
                        op = new PotentialPoint();
                        op = T[Ti, Tj - 1, Tk][0];
                        temp.transform.position = new Vector3(Ti, Tj - 1, 0);
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti, Tj - 1, Tk][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti;
                                tj = Tj-1;
                                ta = Tk;
                            }
                        }
                    }
                    if (true)
                    {
                        int theta = Tk;
                        if (Tk - 4 <= -1)
                        {
                            theta = 356 + Tk;
                            temp.transform.eulerAngles = new Vector3(0, 0, theta);
                        }
                        else
                        {
                            theta = Tk - 4;
                            temp.transform.eulerAngles = new Vector3(0, 0, theta);
                        }
                        op = new PotentialPoint();
                        op = T[Ti, Tj, theta][0];
                        int con1x = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x);
                        int con1y = Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y);
                        int con2x = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x);
                        int con2y = Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y);
                        if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 255 && op.visited == false && Cross(temp))
                        {
                            op.parent = new PotentialPointParent
                            {
                                px = Ti,
                                py = Tj,
                                angle = Tk,
                            };
                            op.visited = true;
                            T[Ti, Tj, theta][0] = op;
                            open[(map1[con1x, con1y] + map2[con2x, con2y]) / 2].Insert(0, op);
                            if ((map1[con1x, con1y] + map2[con2x, con2y]) / 2 < 2)
                            {
                                success = true;
                                ti = Ti;
                                tj = Tj;
                                ta = theta;
                            }
                        }
                    }
                    break;
                }else if (i == 254)
                {
                    haveopen = false;
                }
            }
            if (haveopen == false)
            {
                break;
            }
        }
        //print(count); //debug
        if (success == true)
        {
            print("succes");
            bool drawpath = true;
            int x = Convert.ToInt32(GameObject.Find("final0").transform.position.x);
            int y = Convert.ToInt32(GameObject.Find("final0").transform.position.y);
            int z = Convert.ToInt32(GameObject.Find("final0").transform.eulerAngles.z);
            int fx = x;
            int fy = y;
            int fz = z;
            Config fi = new Config
            {
                px = x,
                py = y,
                angle = z
            };
            run.Add(fi);
            PotentialPoint goal = new PotentialPoint();
            PotentialPointParent gparent = new PotentialPointParent
            {
                px = ti,
                py = tj,
                angle = ta,
            };
            goal = T[x, y, z][0];
            goal.parent = gparent;
            T[x, y, z][0] = goal;
            temp.transform.position = new Vector3(GameObject.Find("final0").transform.position.x, GameObject.Find("final0").transform.position.y, 0);
            temp.transform.eulerAngles = new Vector3(0, 0, GameObject.Find("final0").transform.eulerAngles.z);
            while (drawpath)
            {
                GameObject draw = Instantiate(GameObject.Find("Sphere"));
                //GameObject draw = new GameObject();
                //draw.AddComponent<LineRenderer>();
                //LineRenderer lineRenderer = draw.GetComponent<LineRenderer>();
                //lineRenderer.SetVertexCount();

                x = Convert.ToInt32(T[fx, fy, fz][0].parent.px);
                y = Convert.ToInt32(T[fx, fy, fz][0].parent.py);
                z = Convert.ToInt32(T[fx, fy, fz][0].parent.angle);
                Config pp = new Config
                {
                    px = x,
                    py = y,
                    angle = z
                };
                run.Add(pp);
                fx = x;
                fy = y;
                fz = z;
                draw.transform.position = new Vector3(T[x, y, z][0].px, T[x, y, z][0].py, 0);
                draw.transform.eulerAngles = new Vector3(0, 0, T[x, y, z][0].angle);
                if ((x == initial.px && y == initial.py && z == initial.angle) || (x == 0 && y == 0 & z == 0))
                {
                    break;
                }
            }
        }
        else
        {
            //for (int i = 0; i < path.Count; i++)
            //{
            //    print(path[i].px + path[i].py);
            //}
            //print(map1[Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.x), Convert.ToInt32(GameObject.Find("temprob/controll0").transform.position.y)] + map2[Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.x), Convert.ToInt32(GameObject.Find("temprob/controll1").transform.position.y)]);
            print("No Path");
            for (int i = 0; i < 255; i++)
            {
                if (open[i].Count != 0)
                {
                    print("X");
                }
            }
        }
    }

    bool Cross(GameObject obj)
    {
        for(int i = 0; i < obj.transform.childCount - 2; i++)
        {
            List<Vertice> pver = new List<Vertice>();
            int L = obj.transform.GetChild(i).gameObject.GetComponent<PolygonCollider2D>().points.Length;
            for(int j = 0; j < L; j++)
            {
                Vector2 pv = obj.transform.GetChild(i).gameObject.GetComponent<PolygonCollider2D>().transform.TransformPoint(obj.transform.GetChild(i).gameObject.GetComponent<PolygonCollider2D>().points[j]);
                Vertice ver = new Vertice
                {
                    vx = pv.x,
                    vy = pv.y
                };
                pver.Add(ver);
            }
            for(int k = 0; k < pver.Count; k++)
            {
                Vertice A = new Vertice
                {
                    vx = pver[k % pver.Count].vx,
                    vy = pver[k % pver.Count].vy
                };
                Vertice B = new Vertice
                {
                    vx = pver[(k + 1) % pver.Count].vx,
                    vy = pver[(k + 1) % pver.Count].vy
                };
                for(int m = 0; m < obsnum; m++)
                {
                    for (int n = 0; n < GameObject.Find("obstacle" + m).transform.childCount; n++)
                    {
                        int PL = GameObject.Find("obstacle" + m).transform.GetChild(n).gameObject.GetComponent<PolygonCollider2D>().points.Length;
                        for (int p = 0; p < PL; p++)
                        {
                            Vertice p1 = new Vertice();
                            p1 = A;
                            Vertice p2 = new Vertice();
                            p2 = B;
                            Vector2 op1 = GameObject.Find("obstacle" + m).transform.GetChild(n).gameObject.GetComponent<PolygonCollider2D>().transform.TransformPoint(GameObject.Find("obstacle" + m).transform.GetChild(n).gameObject.GetComponent<PolygonCollider2D>().points[p % PL]);
                            Vector2 op2 = GameObject.Find("obstacle" + m).transform.GetChild(n).gameObject.GetComponent<PolygonCollider2D>().transform.TransformPoint(GameObject.Find("obstacle" + m).transform.GetChild(n).gameObject.GetComponent<PolygonCollider2D>().points[(p + 1) % PL]);
                            Vertice p3 = new Vertice
                            {
                                vx = op1.x,
                                vy = op1.y
                            };
                            Vertice p4 = new Vertice
                            {
                                vx = op2.x,
                                vy = op2.y
                            };
                            if ((((p1.vy - p2.vy) * (p3.vx - p1.vx) + (p2.vx - p1.vx) * (p3.vy - p1.vy)) * ((p1.vy - p2.vy) * (p4.vx - p1.vx) + (p2.vx - p1.vx) * (p4.vy - p1.vy)) < 0) && ((p3.vy - p4.vy) * (p1.vx - p3.vx) + (p4.vx - p3.vx) * (p1.vy - p3.vy)) * ((p3.vy - p4.vy) * (p2.vx - p3.vx) + (p4.vx - p3.vx) * (p2.vy - p3.vy)) < 0)
                            {
                                print("banggggggg");
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    public void ShowPath()
    {
        show = true;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
    
    // Use this for initialization
    void Start()
    {
        ReadRobot();
        ReadObstacle();
    }

    // Update is called once per frame
    bool show=false;
    int r = 0;
    void Update()
    {
        if (show)
        {
            if(run!=null)
            {
                GameObject.Find("temprob").transform.position = new Vector3(run[run.Count - r-1].px, run[run.Count - r-1].py, 0);
                GameObject.Find("temprob").transform.eulerAngles = new Vector3(0, 0, run[run.Count - r-1].angle);
                if (r == run.Count-1)
                {
                    show = false;
                }
                r++;
            }
        }
        else
        {
            r = 0;
        }

        if (Input.GetKey(KeyCode.Space)) {
			Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
            ReadRobot();
            ReadObstacle();
		}

    }
}