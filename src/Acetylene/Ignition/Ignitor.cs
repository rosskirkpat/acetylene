using System.Collections;
using System.DirectoryServices;
using System.Text.Json;
using Serilog;

public class Ignitor {
    public Ignitor() {
    }

    public static IgnitionFile Parse(string contents) {
        var options = new JsonSerializerOptions() {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<IgnitionFile>(contents, options);
    }

    public static void CreateUser(string name, string password, IList groups) {
        try {
            // check if user exists?
            var de = DirectoryEntry($"WinNT://{Environment.MachineName}computer").Children
            var DirectoryEntry newUser = new de.Add(name, "user");
            newUser.Invoke("SetPassword", new object[] { password });
            newUser.Invoke("Put", new object[] { "Description", "Acetylene Created User" });
            newUser.CommitChanges();
            Serilog.Log.Information("Successfully created account:" + name);

            foreach (var group in groups) {
                // check if group exists?
                DirectoryEntry newGroup = new de.Add((string)group, "group");
                if (newGroup != null) {
                    newGroup.Invoke("Add", new object[] { NewUser.Path.ToString() });
                }
                newGroup.CommitChanges();
                Serilog.Log.Information("Successfully created group:" + group);
            }
        } catch {
            Serilog.Log.Error("Encountered error while creating account:" + name);
        }
    }

    public static void AddSshKey(IList keys, string username) {
        Directory.CreateDirectory("C:\\ProgramData\\ssh");

        // if config.PrimaryGroup == "Administrators 
        string path = @"C:\ProgramData\ssh\administrators_authorized_keys";
        // icacls.exe "C:\ProgramData\ssh\administrators_authorized_keys" / inheritance:r / grant "Administrators:F" / grant "SYSTEM:F"
        // else path = @"C:\\Users\\" + username + "\\.ssh";
        // icacls.exe path / inheritance:r / grant "username:F" / grant "SYSTEM:F"

        try {
            if (System.IO.File.Exists(path)) {
            } else {
                System.IO.File.Create(path);
            }
            foreach (var key in keys) {
                var StreamWriter sw = System.IO.File.AppendText(path);
                sw.WriteLine((string)key);
            }

        } catch {
            Serilog.Log.Error("Encountered error while adding ssh keys for account:" + username);
        }
    }
}