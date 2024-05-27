
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Media3D;
using SurfaceAnalyzer;
using System.Numerics;
namespace Tutorial
{
     partial  class Program{

        static int readcount=0;
        static int readed =0;
        static System.IO.StreamReader sr;
        static StreamWriter sw;
        static System.Collections.Generic.List<Vector3D[]> vecl;

        static string fileName;
        static double[] angs;
        static PolygonModel polygonstl;
        private static void DxfRead()
        {
          var sw = new System.Diagnostics.Stopwatch();
  
          sw.Start();
          StartChk();
          sw.Stop();
           Console.WriteLine($"StartChk {sw.ElapsedMilliseconds}");
         
          sw.Restart();

          savestl(); sw.Stop();
          Console.WriteLine($"stl {sw.ElapsedMilliseconds}");

          
          sw.Restart();
          nomal();//こっちはノーマライズしておこう
          sw.Stop();
          Console.WriteLine($"nomalise{sw.ElapsedMilliseconds}");
          
        
                 
        }
        private static void Opend()
        {
        //オープンファイルダイアログを生成する
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "ファイルを開く";
            op.InitialDirectory = @"./";
            op.FileName = @"sample.dxf";
            op.Filter = "dxfファイル(*.dxf;*.DXF)|*.csv;*.dxf|すべてのファイル(*.*)|*.*";
            op.FilterIndex = 1;

            //オープンファイルダイアログを表示する
            DialogResult result = op.ShowDialog();

            if (result == DialogResult.OK)
            {
              //「開く」ボタンが選択された時の処理
              fileName = op.FileName;  //こんな感じで選択されたファイルのパスが取得できる
              DxfRead(); 
            }
            else if (result == DialogResult.Cancel)
            {
              //「キャンセル」ボタンまたは「×」ボタンが選択された時の処理
            }
        }
        private static void readsdf(string fileName)
        {
          
            string line;  
            int counter=0;
            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(fileName);  
           // line = file.ReadLine();
            //if(line!=null){
                    //Vertices = new  float[int.Parse(line)*9];
                    if(readcount==0)readcount =200000;
                    Vertices = new  float[readcount*9];
                    Console.WriteLine(""+Vertices.Length);
            //}
            while((line = file.ReadLine()) != null)  
            {  
                line= file.ReadLine();
                var values = line.Split(",");
                if(values.Length>=4 && counter<Vertices.Length){
                    for(int f=0;f<4;f++){
                        Vertices[counter]   = float.Parse(values[f]);
                        counter++;  
                    }
                }
            }  
           file.Close();
           
        }

        private static void areastl()
        {


          double area = 0;
          int no =0;
          Vector3 vector_ue = new Vector3(0, 1, 0);//上向き
          if(onoff==true){

            foreach (var face in polygonstl.Faces)
            {
              no++;
              if(angs[no] > slider1)
                area += face.Area();
            }
            menseki ="surface area:"+area;     
      

          }else{
            foreach (var face in polygonstl.Faces)
            {
              if(face.FacePoint().Z > slider1)
                area += face.Area();
            }
            menseki ="area:"+area;   
          }

            


            //string stlpath = @"D:/共有/stl/";
         
            /*STL*/
            //polygonstl = new PolygonModel(vecl,true);
            //SurfaceAnalyzer.SaveData.SaveSTL(polygonstl, stlpath,"teststl");
            
        }
        private static void savestl()
        {
            //string stlpath = @"D:/共有/stl/";
         
            /*STL*/
           
   
            // Vector3[] dd_vecl = new Vector3[1];  

           // polygonstl = new PolygonModel(dd_vecl,true);
            //SurfaceAnalyzer.SaveData.SaveSTL(polygonstl, stlpath,"teststl");
        }
        private static void nomal()
        {
            Vector3D max_vec  = new Vector3D();
            Vector3D min_vec  = new Vector3D();
            Vector3D nom_vec  = new Vector3D();
            Vector3D wrt_vec  = new Vector3D();
       
            max_vec.X= min_vec.X =vecl[0][0].X;
            max_vec. Y= min_vec.Y =vecl[0][0].Y;
            max_vec.Z= min_vec.Z =vecl[0][0].Z;
            double min_value = vecl[0][0].X;
            double max_value = vecl[0][0].X;
            double nom_value;
            string rwpath = @"./vertics.csv";
            
            sw = new StreamWriter(rwpath, false,Encoding.GetEncoding("Shift_JIS"));
            //sw.WriteLine(readcount+"");
            
            //まずはMaxMin
            for(int i=0;i<vecl.Count;i++){
              for(int j=0;j<3;j++){
                /*x,y,z個別にMAXMINを出す  */
                  max_vec.X = (vecl[i][j].X>max_vec.X)?vecl[i][j].X:max_vec.X;
                  min_vec.X = (vecl[i][j].X<min_vec.X)?vecl[i][j].X:min_vec.X;

                  max_vec.Y = (vecl[i][j].Y>max_vec.Y)?vecl[i][j].Y:max_vec.Y;
                  min_vec.Y = (vecl[i][j].Y<min_vec.Y)?vecl[i][j].Y:min_vec.Y;

                  max_vec.Z = (vecl[i][j].Z>max_vec.Z)?vecl[i][j].Z:max_vec.Z;
                  min_vec.Z = (vecl[i][j].Z<min_vec.Z)?vecl[i][j].Z:min_vec.Z;
                
                  /*共通で 
                  max_value = (vecl[i][j].X>max_value)?vecl[i][j].X:max_value;
                  min_value = (vecl[i][j].X<min_value)?vecl[i][j].X:min_value;

                  max_value = (vecl[i][j].Y>max_value)?vecl[i][j].Y:max_value;
                  min_value = (vecl[i][j].Y<min_value)?vecl[i][j].Y:min_value;

                  max_value = (vecl[i][j].Z>max_value)?vecl[i][j].Z:max_value;
                  min_value = (vecl[i][j].Z<min_value)?vecl[i][j].Z:min_value;
                 */
              }
            }

           nom_vec.X = max_vec.X-min_vec.X;
           nom_vec.Y = max_vec.Y-min_vec.Y;
           nom_vec.Z = max_vec.Z-min_vec.Z;

            nom_value = max_value - min_value;
            Vector3D vector_ue = new Vector3D(0, 1, 0);//上向き

            angs = new double[vecl.Count];
            for(int i=0;i<vecl.Count;i++){
              double ang = Vector3D.AngleBetween(Normal(vecl[i][0],vecl[i][1]),vector_ue);//法線ベクトルと上向きの角
              angs[i]=ang;

              for(int j=0;j<3;j++){
                /*x,y,z個別にノーマライズ*/
                /* */
                wrt_vec.X = (max_vec.X-vecl[i][j].X)/nom_vec.X;
                wrt_vec.Y = (max_vec.Y-vecl[i][j].Y)/nom_vec.Y;
                wrt_vec.Z = (max_vec.Z-vecl[i][j].Z)/nom_vec.Z;
               
                /*全点一緒に 
                wrt_vec.X = (max_value-vecl[i][j].X)/nom_value;
                wrt_vec.Y = (max_value-vecl[i][j].Y)/nom_value;
                wrt_vec.Z = (max_value-vecl[i][j].Z)/nom_value;
                
                 */

                //var face = polygonstl.Faces[i*3+j];//１枚取り出して法線ベクトル
                // Vector3D normal = face.Normal();

                sw.WriteLine(wrt_vec.X+","+wrt_vec.Y+","+wrt_vec.Z+","+ang);

              }
            }


/* ノーマライズしないで単に保存
            for(int i=0;i<vecl.Count;i++){
              for(int j=0;j<3;j++){
                wrt_vec.X = vecl[i][j].X;
                wrt_vec.Y = vecl[i][j].Y;
                wrt_vec.Z = vecl[i][j].Z;
                sw.WriteLine(wrt_vec.X+","+wrt_vec.Y+","+wrt_vec.Z+",");

              }
            }
*/
 
            Console.WriteLine("x max"+ max_vec.X+" min"+min_vec.X+" ="+ nom_vec.X);
            Console.WriteLine("y max"+ max_vec.Y+" min"+min_vec.Y+" ="+ nom_vec.Y);
            Console.WriteLine("z max"+ max_vec.Z+" min"+min_vec.Z+" ="+ nom_vec.Z);
            sw.Close();
        }
        private static Vector3D  Normal(Vector3D  a, Vector3D b) {
  
              var dir =  Vector3D.CrossProduct(a,b);
            
               dir.Normalize();
              return dir;
     
        }
       

        private static void StartChk()
        {
 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            sr = new System.IO.StreamReader(fileName,Encoding.GetEncoding("Shift_JIS"));  

 
             /*STL*/
           
            vecl = new System.Collections.Generic.List<Vector3D[]>() ;
            string line;
            while((line = sr.ReadLine()) != null)  
            {  
  
                if(line =="ENTITIES"){
                   findENTITIES();
                }
            }


            CloseFile();
            string filePath = Path.GetFileName(fileName);
            hhh = System.Text.Encoding.UTF8.GetBytes(filePath);
        }
        private static void findENTITIES()
        {
             Console.WriteLine("I can find ENTITIES.");
            // comments = "I can find dxf file.";
            string line;int count=0;
            while((line = sr.ReadLine()) != null)  
            { 
                if(line =="3DFACE"){
                   find3dface();
                }
                count++;
                
            }

             Console.WriteLine("I have finished!!");
             //comments = "I have finished!!";
        }
        private static void find3dface()
        {
            string line;
            int flgx0=0,flgy0=0,flgz0=0;
            int flgx1=0,flgy1=0,flgz1=0;
            int flgx2=0,flgy2=0,flgz2=0;
            int flgx3=0,flgy3=0,flgz3=0;

            double valx0=0.0,valy0=0.0,valz0=0.0;
            double valx1=0.0,valy1=0.0,valz1=0.0;
            double valx2=0.0,valy2=0.0,valz2=0.0;
            double valx3=0.0,valy3=0.0,valz3=0.0;

            while((line = sr.ReadLine()) != null)  
            { 


                if(line ==" 10"){
                  valx0=getvalue();
                  flgx0 = 1;
                }
                if(line ==" 20"){
                  valy0=getvalue();
                  flgy0 = 1;
                }
                if(line ==" 30"){
                  valz0=getvalue();
                  flgz0 = 1;
                }

                if(line ==" 11"){
                  valx1=getvalue();
                  flgx1 = 1;
                }
                if(line ==" 21"){
                  valy1=getvalue();
                  flgy1 = 1;
                }
                if(line ==" 31"){
                  valz1=getvalue();
                  flgz1 = 1;
                }
                
                if(line ==" 12"){
                  valx2=getvalue();
                  flgx2 = 1;
                }
                if(line ==" 22"){
                  valy2=getvalue();
                  flgy2 = 1;
                }
                if(line ==" 32"){
                  valz2=getvalue();
                  flgz2 = 1;
                }

                if(line ==" 13"){
                  valx3=getvalue();
                  flgx3 = 1;
                }
                if(line ==" 23"){
                  valy3=getvalue();
                  flgy3 = 1;
                }
                if(line ==" 33"){
                  valz3=getvalue();
                  flgz3 = 1;
                }

                if(line =="3DFACE"){//もうない
                  break;
                }
                if(flgx0+flgy0+flgz0 + flgx1+flgy1+flgz1 + flgx2+flgy2+flgz2 + flgx3+flgy3+flgz3==12)
                {
                    /*csv
                    sw.WriteLine(valx0.ToString("f")+","+valy0.ToString("f")+","+valz0.ToString("f"));
                    sw.WriteLine(valx1.ToString("f")+","+valy1.ToString("f")+","+valz1.ToString("f"));
                    sw.WriteLine(valx2.ToString("f")+","+valy2.ToString("f")+","+valz2.ToString("f"));
                    */
                    
                    /*おれがかってにやったノーマライズ
                    Vector3D vec1 = new Vector3D(valx0/10-4850,valy0/10-16980,valz0/10-24);
                    Vector3D vec2 = new Vector3D(valx1/10-4850,valy1/10-16980,valz1/10-24);
                    Vector3D vec3 = new Vector3D(valx2/10-4850,valy2/10-16980,valz2/10-24);
                    Vector3D[] vecs  = new Vector3D[3];
                    */
                    Vector3D[] vecs  = new Vector3D[3];
                    Vector3D vec1 = new Vector3D(valx0,valy0,valz0);
                    Vector3D vec2 = new Vector3D(valx1,valy1,valz1);
                    Vector3D vec3 = new Vector3D(valx2,valy2,valz2);
                    
                    
                    vecs[0]= vec1;
                    vecs[1]= vec2;
                    vecs[2]= vec3;
                   
                    vecl.Add(vecs);
                    
                    //
                  System.Numerics.Vector3 vec3_1 = new System.Numerics.Vector3((float)valx0,(float)valy0,(float)valz0);
                  System.Numerics.Vector3 vec3_2 = new System.Numerics.Vector3((float)valx1,(float)valy1,(float)valz1);
                  System.Numerics.Vector3 vec3_3 = new System.Numerics.Vector3((float)valx2,(float)valy2,(float)valz2);
                  System.Numerics.Vector3[] vecs3  = new System.Numerics.Vector3[3];
                  vecs3[0]= vec3_1;
                  vecs3[1]= vec3_2; 
                  vecs3[2]= vec3_3;
                  System.Collections.Generic.List<System.Numerics.Vector3[]> vecstl = new System.Collections.Generic.List<System.Numerics.Vector3[]>() ;
                  vecstl.Add(vecs3);
                    //
                  polygonstl = new PolygonModel(vecstl,true);
                    //
                   flgx0=0;flgy0=0;flgz0=0;
                   flgx1=0;flgy1=0;flgz1=0;
                   flgx2=0;flgy2=0;flgz2=0;
                   flgx3=0;flgy3=0;flgz3=0;

                    /**/
                    readcount++;
                    /*四角形の反対側
                    Vector3D vec4 = new Vector3D(valx2,valy2,valz2);
                    Vector3D vec5 = new Vector3D(valx3,valy3,valz3);
                    Vector3D vec6 = new Vector3D(valx1,valy1,valz1);
                    vecs[0]= vec4;
                    vecs[1]= vec5;
                    vecs[2]= vec6;
                    vecl.Add(vecs);
  
                    
                    readcount++;
                    */

                    return;
                }
            }
            Console.WriteLine("Thre is not enought point in faces .");
            CloseFile();
        }
        private static double getvalue()
        {
            string line;
            if((line = sr.ReadLine()) != null) {
                return double.Parse(line);
                
            }else{
                Console.WriteLine("Thre is no  value faces.");
                CloseFile();
                return -9999;
            }
           
        }
        private static void CloseFile()
        {
            sr.Close();
            //sw.Close();
        }

    }
}
