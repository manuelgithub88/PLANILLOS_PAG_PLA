using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

// <publicacion borrar="True" ready="True" date="20140624" milenium="MILENIUMP" pubname="LVNP" alt_pubname="">


class MainClass {
	
	public  static bool IsFileLocked(String strFile)
{
    FileStream stream = null;
    FileInfo file=new FileInfo(strFile);
    try
    {
        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
    }
    catch (IOException)
    {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)

        return true;
    }
    finally
    {
        if (stream != null)
            stream.Close();
    }

    //file is not locked
    return false;
}


  private static int DisplayMatches(string text,string regularExpressionString) 
  {
  	int iSuccess=0;
    Console.WriteLine("Probando Regex: " +regularExpressionString);
    MatchCollection myMatchCollection = Regex.Matches(text, regularExpressionString);
    foreach (Match myMatch in myMatchCollection) {
      Console.WriteLine(myMatch);
      iSuccess++;
    }
    
    	return iSuccess;
  }

  public static void Main(String [] args) 
  {
  	
  	
    if (args.Length < 2)
    	{
    	Console.WriteLine("Uso ");
    	Console.WriteLine("filtra_publicaciones <REG_EXP/FICHERO_CONF> <FICHERO_DIR> <DIRECTORIO_MATCHEO> <DIRECTORIO_NOMATCHEO> ");
    	Environment.Exit(2);
    	}

    
    Console.WriteLine("FICHERO:"+args[1]);
    if (!Directory.Exists(args[1]))
    	{
    	Console.WriteLine("No existe Fichero;"+args[1]);
    	Environment.Exit(2);
    	}
    
    string [] fileEntries = Directory.GetFiles(args[1],"*.xml");
    
    if (!File.Exists(args[0]))
        {
    	foreach(string fileName in fileEntries)
    		{
    		// <publicacion borrar="True" ready="True" date="20140624" milenium="MILENIUMP" pubname="LVNP" alt_pubname="">
			String strFile=File.ReadAllText(fileName);    	
		
			if (!strFile.Contains("ready=\"True\""))
				{
				Console.WriteLine("NO  ESTA LISTO AUN 2");
			
				continue;
				}
		// bucle conreglas
    		if (DisplayMatches(fileName, args[0])>0)
    			{
    			Console.WriteLine("Dir DESTINO "+args[2]);
    			File.Move(fileName,args[2]+"\\"+Path.GetFileName(fileName));
    			
    			// Despues del move salir, sino el move
    			}
    		else
    			{
    			Console.WriteLine("Dir DESTINO "+args[3]);
    			File.Copy(fileName,args[3]+"\\"+Path.GetFileName(fileName),true);
    			File.Delete(fileName);
    			}
    		}
        }
       else
        {
       	    string[] readLines = File.ReadAllLines(args[0]);
	    	foreach(string fileName in fileEntries)
    		{
    		// <publicacion borrar="True" ready="True" date="20140624" milenium="MILENIUMP" pubname="LVNP" alt_pubname="">
			String strFile=File.ReadAllText(fileName);    					
			if (!strFile.Contains("ready=\"True\""))
				{
				Console.WriteLine("NO  ESTA LISTO AUN 2222");
				Console.WriteLine(args[1]+"\\COPIA\\"+Path.GetFileName(fileName));

				File.Copy(fileName,args[1]+"\\COPIA\\"+Path.GetFileName(fileName),true);
				
				continue;
				}
			
		// bucle conreglas
		
			foreach (String strLine in readLines)
				{
				if (strLine.StartsWith("#"))
					continue;
				String [] strPiezas=strLine.Split(new Char [] {';' , '@' });
				
				if (DisplayMatches(fileName, /*args[0]*/ strPiezas[0])>0)
    				{
    				Console.WriteLine("Dir DESTINO "+strPiezas[1]);
    				if (IsFileLocked(fileName))
    					continue;
    					
    				File.Copy(fileName,strPiezas[1]+"\\"+Path.GetFileName(fileName),true);
    				File.Delete(fileName);    			
    				try {
    					Thread.Sleep(2500);
    					Console.WriteLine("PROPCEDE A COPIAR" +args[1]+"\\COPIA\\"+Path.GetFileName(fileName) + "SOBRE:" +fileName);
    					File.Copy(args[1]+"\\COPIA\\"+Path.GetFileName(fileName),fileName,true);
    				} catch (Exception e) { Console.WriteLine("YA ESTA EL FICHERO ALLI" +e.ToString());}
    				
    				break;
    					
    				}       	
        	}
  		}
    }
  }
}
