Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemTypes
    Friend Const Key = "Key"
    Friend Const GoblinCorpse = "GoblinCorpse"
    Friend Const Meat = "Meat"
    Friend Const RottenMeat = "RottenMeat"
    Friend Const Club = "Club"
    Friend Const WoodenShield = "WoodenShield"
    Friend Const BikiniTop = "BikiniTop"
    Friend Const Thong = "Thong"
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
                Club,
                New ItemTypeDescriptor(
                    "Club",
                    ChrW(10),
                    Brown,
                    equipSlotType:=EquipSlotTypes.Weapon,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 5},
                        {StatisticTypes.Durability, 10},
                        {StatisticTypes.MaximumDurability, 10},
                        {StatisticTypes.AttackDice, 2},
                        {StatisticTypes.MaximumAttack, 0}
                    })
            },
            {
                BikiniTop,
                New ItemTypeDescriptor(
                    "Bikini Top",
                    ChrW(12),
                    Pink,
                    equipSlotType:=EquipSlotTypes.Chest,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.MaximumEncumbrance, 10}
                    })
            },
            {
                Thong,
                New ItemTypeDescriptor(
                    "Thong",
                    ChrW(13),
                    Pink,
                    equipSlotType:=EquipSlotTypes.Pelvis,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.MaximumEncumbrance, 5}
                    })
            },
            {
                WoodenShield,
                New ItemTypeDescriptor(
                    "Wooden Shield",
                    ChrW(11),
                    Brown,
                    equipSlotType:=EquipSlotTypes.Shield,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 5},
                        {StatisticTypes.Durability, 10},
                        {StatisticTypes.MaximumDurability, 10},
                        {StatisticTypes.DefendDice, 2},
                        {StatisticTypes.MaximumDefend, 1}
                    })
            },
            {
                Meat,
                New ItemTypeDescriptor(
                    "Meat",
                    ChrW(9),
                    Red,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 0}
                    },
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Eat, AddressOf EatMeat}
                    })
            },
            {
                RottenMeat,
                New ItemTypeDescriptor(
                    "Meat",
                    ChrW(9),
                    Red,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Encumbrance, 0}
                    },
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Eat, AddressOf EatRottenMeat}
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

    Private Sub EatRottenMeat(character As ICharacter, item As IItem)
        Dim message = character.World.CreateMessage.AddLine(LightGray, $"{character.Name} eats {item.Name}")
        character.RemoveItem(item)
        item.Recycle()
        message.AddLine(Red, $"{item.Name} is rotten!")
        message.AddLine(LightGray, $"{character.Name} takes 1 damage!")
        character.SetHealth(character.Health - 1)
        If character.IsTitsUp Then
            character.Metadata(Metadatas.Epitaph) = $"Be careful which meat you put in yer mouth!"
            message.AddLine(LightGray, $"{character.Name} is tits up!").SetSfx(Sfx.PlayerDeath)
        Else
            message.AddLine(LightGray, $"{character.Name} has {character.Health} health left!").SetSfx(Sfx.PlayerHit)
        End If
    End Sub

    Private Sub EatMeat(character As ICharacter, item As IItem)
        Dim message = character.World.CreateMessage.AddLine(LightGray, $"{character.Name} eats {item.Name}").SetSfx(Sfx.Tasty)
        character.RemoveItem(item)
        item.Recycle()
        message.AddLine(Green, $"{character.Name} regains 1 health!")
        character.SetHealth(character.Health + 1)
        message.AddLine(LightGray, $"{character.Name} has {character.Health} health!")
    End Sub

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
