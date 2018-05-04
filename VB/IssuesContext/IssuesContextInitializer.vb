Imports System
Imports System.Data.Entity
Imports System.Linq

Namespace InfiniteAsyncSourceEFSample
    Public Class IssuesContextInitializer
        Inherits DropCreateDatabaseIfModelChanges(Of IssuesContext)

        ': DropCreateDatabaseAlways<IssuesContext> { 

        Protected Overrides Sub Seed(ByVal context As IssuesContext)
            MyBase.Seed(context)
            Dim users = OutlookDataGenerator.Users.Select(Function(x)
                Dim split = x.Split(" "c)
                Return New User() With { _
                    .FirstName = split(0), _
                    .LastName = split(1) _
                }
            End Function).ToArray()
            context.Users.AddRange(users)
            context.SaveChanges()

            Dim rnd = New Random()
            Dim issues = Enumerable.Range(0, 1000).Select(Function(i) New Issue() With { _
                .Subject = OutlookDataGenerator.GetSubject(), _
                .UserId = users(rnd.Next(users.Length)).Id, _
                .Created = Date.Today.AddDays(-rnd.Next(30)), _
                .Priority = OutlookDataGenerator.GetPriority(), _
                .Votes = rnd.Next(100) _
            }).ToArray()
            context.Issues.AddRange(issues)

            context.SaveChanges()
        End Sub
    End Class
End Namespace
