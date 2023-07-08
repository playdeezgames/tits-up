Imports System.Runtime.CompilerServices

Friend Module VerbTypes
    Friend Const Eat = "Eat"
    Friend Const TakeStick = "TakeStick"
    Friend Const Apply = "Apply"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, VerbDescriptor) =
        New Dictionary(Of String, VerbDescriptor) From
        {
            {
                Eat,
                New VerbDescriptor(
                    "Eat")
            },
            {
                Apply,
                New VerbDescriptor(
                    "Apply")
            },
            {
                TakeStick,
                New VerbDescriptor(
                    "Take Stick")
            }
        }
    <Extension>
    Friend Function ToVerbTypeDescriptor(verbType As String) As VerbDescriptor
        Return descriptors(verbType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
