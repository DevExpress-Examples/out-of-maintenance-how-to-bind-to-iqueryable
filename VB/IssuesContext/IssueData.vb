Imports System
Imports System.Linq

Namespace InfiniteAsyncSourceEFSample
    Public Class IssueData
        Public Shared Function [Select](ByVal issues As IQueryable(Of Issue)) As IQueryable(Of IssueData)
            Return issues.Select(Function(x) New IssueData() With { _
                .Id = x.Id, _
                .Subject = x.Subject, _
                .User = x.User.FirstName & " " & x.User.LastName, _
                .Created = x.Created, _
                .Votes = x.Votes, _
                .Priority = x.Priority _
            })
        End Function
        Private privateId As Integer
        Public Property Id() As Integer
            Get
                Return privateId
            End Get
            Private Set(ByVal value As Integer)
                privateId = value
            End Set
        End Property
        Private privateSubject As String
        Public Property Subject() As String
            Get
                Return privateSubject
            End Get
            Private Set(ByVal value As String)
                privateSubject = value
            End Set
        End Property
        Private privateCreated As Date
        Public Property Created() As Date
            Get
                Return privateCreated
            End Get
            Private Set(ByVal value As Date)
                privateCreated = value
            End Set
        End Property
        Private privateVotes As Integer
        Public Property Votes() As Integer
            Get
                Return privateVotes
            End Get
            Private Set(ByVal value As Integer)
                privateVotes = value
            End Set
        End Property
        Private privatePriority As Priority
        Public Property Priority() As Priority
            Get
                Return privatePriority
            End Get
            Private Set(ByVal value As Priority)
                privatePriority = value
            End Set
        End Property

        Private privateUser As String
        Public Property User() As String
            Get
                Return privateUser
            End Get
            Private Set(ByVal value As String)
                privateUser = value
            End Set
        End Property
    End Class
End Namespace
