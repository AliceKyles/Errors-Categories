using System;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Text;
using System.Threading;

class Program
{
	public static void Main(string[] args)
	{
		SqlConnection conn = new SqlConnection();
		try {
			conn.ConnectionString = "Data Source=DESKTOP-0NFOTH7;" +
			"Initial Catalog=master;" +
			"Integrated Security=SSPI";
			conn.Open();
			Console.WriteLine("Opened database");
		} catch {
			Console.WriteLine("Couldn't open database");
			Console.ReadLine();
			Environment.Exit(0);
		}
		Thread Thread1 = new Thread(()=>ErrorCode(conn)); //Создаем новый объект потока (Thread)
		Thread1.Start();
		Thread Thread2 = new Thread(()=>Categories(conn)); //Создаем новый объект потока (Thread)
		Thread2.Start();
		Console.ReadLine();
		conn.Close();
	}
	
	public static void ErrorCode(SqlConnection conn)
	{
		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
		WebClient webClient = new WebClient();
		cmd.Parameters.Add("@code", SqlDbType.Int);
		cmd.Parameters.Add("@message", SqlDbType.VarChar);
		byte[] myDataBuffer;
		string download = "";
		try {		
			myDataBuffer = webClient.DownloadData("https://pastebin.com/raw/JK7WiMax");
			download = Encoding.UTF8.GetString(myDataBuffer);
			Console.WriteLine("Loaded file with error codes");
		} catch {
			Console.WriteLine("Couldn't load file with error codes");
			Console.ReadLine();
			Environment.Exit(0);
		}
		download = download.Replace("<ErrorCodes>", "");
		download = download.Replace("</ErrorCodes>", "");
		int count = 0, succes = 0;
		while (download.IndexOf("/>") != -1) {
			count++;
			int[] A  = new int[2];
			A=param(download,"code=");
			int[] B  = new int[2];
			B=param(download,"text=");
			if (check(A)&& check(B)) {
				cmd.Parameters["@code"].Value = download.Substring(A[0], A[1]-A[0]);
				cmd.Parameters["@message"].Value = download.Substring(B[0], B[1]-B[0]);
			}
			if (download.IndexOf("/>") >= download.Length - 2)
				download.Remove(0);
			else
				download = download.Substring(download.IndexOf("/>") + 2);
			try {
				cmd.CommandText = @"INSERT INTO dbo.ErrorStage(ErrorCode,ErrorMessage) VALUES(@code, @message)";
				cmd.ExecuteNonQuery();
				succes++;
			} catch {
				Console.WriteLine("Couldn't insert {0},{1}",cmd.Parameters["@code"].Value,cmd.Parameters["@message"].Value);
			}
		}
		Console.WriteLine("{0} out of {1} error codes were inserted", succes, count);
		try {
			cmd.CommandText = "EXECUTE dbo.ErrorCodes;";
			cmd.ExecuteNonQuery();
			Console.WriteLine("Error codes processed");
		} catch {
			Console.WriteLine("Couldn't process error codes");
		}
	}
	
	public static void Categories(SqlConnection conn)
	{
		SqlCommand cmd = new SqlCommand();
		cmd.Connection = conn;
		WebClient webClient = new WebClient();
		cmd.Parameters.Add("@ID", SqlDbType.Int);
		cmd.Parameters.Add("@name", SqlDbType.VarChar);
		cmd.Parameters.Add("@parent", SqlDbType.Int);
		cmd.Parameters.Add("@image", SqlDbType.VarChar);
		byte[] myDataBuffer;
		string download = "";
		try {		
			myDataBuffer = webClient.DownloadData("https://pastebin.com/raw/0RpLbQ19");
			download = Encoding.UTF8.GetString(myDataBuffer);
			Console.WriteLine("Loaded file with categories");
		} catch {
			Console.WriteLine("Couldn't load file with categories");
			Console.ReadLine();
			Environment.Exit(0);
		}
		download = download.Replace("<Categories>", "");
		download = download.Replace("</Categories>", "");
		int count = 0, succes = 0;
		while (download.IndexOf("/>") != -1) {
			count++;
			int[] A  = new int[2];
			A=param(download,"id=");
			int[] B  = new int[2];
			B=param(download,"name=");
			int[] C  = new int[2];
			C=param(download,"parent=");
			int[] D  = new int[2];
			D=param(download,"image=");
			if (check(A)&& check(B) && check(C) && check(D)) {
				cmd.Parameters["@ID"].Value = download.Substring(A[0], A[1] - A[0]);
				cmd.Parameters["@name"].Value = download.Substring(B[0], B[1] - B[0]);
				cmd.Parameters["@parent"].Value = download.Substring(C[0], C[1] - C[0]);
				cmd.Parameters["@image"].Value = download.Substring(D[0], D[1] - D[0]);
			}
			if (download.IndexOf("/>") >= download.Length - 2)
				download.Remove(0);
			else
				download = download.Substring(download.IndexOf("/>") + 2);
			try {
				cmd.CommandText = @"INSERT INTO dbo.CategoriesStage(ID, CategoryName,Parent,CategoryImage) VALUES(@ID, @name,@parent,@image)";
				cmd.ExecuteNonQuery();
				succes++;
			} catch {
				Console.WriteLine("Couldn't insert {0},{1},{2},{3}",cmd.Parameters["@ID"].Value,cmd.Parameters["@name"].Value,cmd.Parameters["@parent"].Value,cmd.Parameters["@image"].Value);
			}
		}
		Console.WriteLine("{0} out of {1} categories were inserted", succes, count);
		try {
			cmd.CommandText = "EXECUTE dbo.Category;";
			cmd.ExecuteNonQuery();
			Console.WriteLine("Categories processed");
		} catch {
			Console.WriteLine("Couldn't process categories");
		}
	}
	
	public static int[] param(string download, string param){
		int[] A  = new int[2];
		A[0] = download.IndexOf(param) + param.Length+1;
			if (A[0] >= 0 && A[0] <= download.Length)
				A[1] = download.IndexOf('"', A[0]);
			else
				A[1] = -1;
			return A;
	}
	
	public static bool check(int[] A){
		if (A[0]==-1||A[1]==-1||A[1]<A[0])
			return false;
		else return true;
	}
}