using ForestGame.Core;

using var proc = System.Diagnostics.Process.GetCurrentProcess();
proc.EnableRaisingEvents = true;
proc.Exited += (object? sender, EventArgs e) => Internals.ProcessExited();

using var client = new Game1();
client.Run();
