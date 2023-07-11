Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemTypes
    Friend Const Key = "Key"
    Friend Const GoblinCorpse = "GoblinCorpse"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {
                Key,
                New ItemTypeDescriptor(
                    "Key",
                    ChrW(8),
                    Orange,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 0}
                    })
            },
            {
                GoblinCorpse,
                New ItemTypeDescriptor(
                    "Goblin Corpse",
                    ChrW(6),
                    Green,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 25}
                    })
            }
        }
    <Extension>
    Friend Function ToItemTypeDescriptor(itemType As String) As ItemTypeDescriptor
        Return descriptors(itemType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
