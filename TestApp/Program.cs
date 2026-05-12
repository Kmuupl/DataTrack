using System;
using System.Collections.Generic;
using System.Threading;

Random rnd = new Random();

var client1Commands = new List<(string, int)>
{
    ("LOGIN admin 1234", rnd.Next(100, 500)),
    ("BALANCE", rnd.Next(100, 500)),
    ("DEPOSIT 300", rnd.Next(100, 500)),
    ("TRANSFER user 150", rnd.Next(100, 500)),
    ("BALANCE", rnd.Next(100, 500)),
    ("LOGS", rnd.Next(100, 500)),
    ("LOGOUT", rnd.Next(100, 500))
};

var client2Commands = new List<(string, int)>
{
    ("LOGIN user pass",     rnd.Next(100, 500)),
    ("BALANCE",             rnd.Next(100, 500)),
    ("DEPOSIT 200",         rnd.Next(100, 500)),
    ("WITHDRAW 100",        rnd.Next(100, 500)),
    ("TRANSFER admin 50",   rnd.Next(100, 500)),
    ("BALANCE",             rnd.Next(100, 500)),
    ("LOGOUT",              rnd.Next(100, 500)),
};

var client3Commands = new List<(string, int)>
{
    ("LOGIN admin 1234",    rnd.Next(100, 500)),
    ("LISTUSERS",           rnd.Next(100, 500)),
    ("ADDUSER testuser abc",rnd.Next(100, 500)),
    ("LISTUSERS",           rnd.Next(100, 500)),
    ("DELETEUSER testuser", rnd.Next(100, 500)),
    ("LISTUSERS",           rnd.Next(100, 500)),
    ("LOGOUT",              rnd.Next(100, 500)),
};

var t1 = new Thread(() => new TestClient("CLIENT-1", "localhost", 8080, client1Commands).Run());
var t2 = new Thread(() => new TestClient("CLIENT-2", "localhost", 8080, client2Commands).Run());
var t3 = new Thread(() => new TestClient("CLIENT-3", "localhost", 8080, client3Commands).Run());

Console.WriteLine("Starting test clients :)");
t1.Start();
Thread.Sleep(50);
t2.Start();
Thread.Sleep(50);
t3.Start();

t1.Join();
t2.Join();
t3.Join();

Console.WriteLine();
Console.WriteLine("All clients finished.");