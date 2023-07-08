Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports TU.Persistence

Friend Module ItemTypes
    Friend Const SnekCorpse = "SnekCorpse"
    Friend Const Stick = "Stick"
    Friend Const Rock = "Rock"
    Friend Const SharpRock = "SharpRock"
    Friend Const Skin = "Skin"
    Friend Const Meat = "Meat"
    Friend Const Fiber = "Fiber"
    Friend Const Twine = "Twine"
    Friend Const Axe = "Axe"
    Friend Const Moss = "Moss"
    Friend Const Bandage = "Bandage"
    Friend Const Shield = "Shield"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, ItemTypeDescriptor) =
        New Dictionary(Of String, ItemTypeDescriptor) From
        {
            {
                SnekCorpse,
                New ItemTypeDescriptor(
                    "Snek Corpse",
                    "2"c,
                    Hue.Tan,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Eat, AddressOf GenericEat}
                    },
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Satiety, 20}
                    })
            },
            {
                Fiber,
                New ItemTypeDescriptor(
                    "Fiber",
                    ChrW(&H5D),
                    Hue.LightGreen)
            },
            {
                Shield,
                New ItemTypeDescriptor(
                    "Shield",
                    ChrW(&H7D),
                    Hue.Brown,
                    equipSlotType:=EquipSlotTypes.Shield,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Durability, 50},
                        {StatisticTypes.MaximumDurability, 50},
                        {StatisticTypes.MinimumDefend, 10},
                        {StatisticTypes.MaximumDefend, 10}
                    })
            },
            {
                Bandage,
                New ItemTypeDescriptor(
                    "Bandage",
                    ChrW(&H57),
                    Hue.Tan,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Apply, AddressOf GenericApply}
                    },
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 10}
                    })
            },
            {
                Moss,
                New ItemTypeDescriptor(
                    "Moss",
                    ChrW(&H96),
                    Hue.Cyan,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Apply, AddressOf GenericApply}
                    },
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Health, 5}
                    })
            },
            {
                Twine,
                New ItemTypeDescriptor(
                    "Twine",
                    ChrW(&H90),
                    Hue.Tan)
            },
            {
                Skin,
                New ItemTypeDescriptor(
                    "Skin",
                    ChrW(&H95),
                    Hue.Tan)
            },
            {
                Meat,
                New ItemTypeDescriptor(
                    "Meat",
                    ChrW(&H9A),
                    Hue.Red,
                    verbTypes:=New Dictionary(Of String, Action(Of ICharacter, IItem)) From
                    {
                        {VerbTypes.Eat, AddressOf GenericEat}
                    },
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Satiety, 25}
                    })
            },
            {
                Stick,
                New ItemTypeDescriptor(
                    "Stick",
                    "T"c,
                    Hue.Brown,
                    equipSlotType:=EquipSlotTypes.Weapon,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Durability, 50},
                        {StatisticTypes.MaximumDurability, 50},
                        {StatisticTypes.MinimumAttack, 5},
                        {StatisticTypes.MaximumAttack, 5}
                    })
            },
            {
                Axe,
                New ItemTypeDescriptor(
                    "Axe",
                    ChrW(&H5F),
                    Hue.LightGray,
                    equipSlotType:=EquipSlotTypes.Weapon,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Durability, 125},
                        {StatisticTypes.MaximumDurability, 125},
                        {StatisticTypes.MinimumAttack, 15},
                        {StatisticTypes.MaximumAttack, 15}
                    })
            },
            {
                Rock,
                New ItemTypeDescriptor(
                    "Rock",
                    ChrW(&HAC),
                    Hue.DarkGray,
                    equipSlotType:=EquipSlotTypes.Weapon,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Durability, 100},
                        {StatisticTypes.MaximumDurability, 100},
                        {StatisticTypes.MinimumAttack, 5},
                        {StatisticTypes.MaximumAttack, 5}
                    })
            },
            {
                SharpRock,
                New ItemTypeDescriptor(
                    "Sharp Rock",
                    ChrW(&HAD),
                    Hue.DarkGray,
                    equipSlotType:=EquipSlotTypes.Weapon,
                    statistics:=New Dictionary(Of String, Integer) From
                    {
                        {StatisticTypes.Durability, 75},
                        {StatisticTypes.MaximumDurability, 75},
                        {StatisticTypes.MinimumAttack, 10},
                        {StatisticTypes.MaximumAttack, 10}
                    })
            }
        }

    Private Sub GenericApply(character As ICharacter, item As IItem)
        Dim message = character.World.CreateMessage.AddLine(LightGray, $"{character.Name} applies {item.Name}")
        Dim healthGain = item.Statistic(StatisticTypes.Health)
        message.AddLine(LightGray, $"{character.Name} gains {healthGain} health")
        character.SetHealth(character.Health + healthGain)
        message.AddLine(LightGray, $"{character.Name} now has {character.Health}/{character.MaximumHealth} health.")
        character.RemoveItem(item)
        item.Recycle()
    End Sub

    Private Sub GenericEat(character As ICharacter, item As IItem)
        character.RemoveItem(item)
        item.Recycle()
        character.SetSatiety(character.Satiety + item.Satiety)
        character.World.CreateMessage().
            AddLine(LightGray, $"You eat the {item.Name}!").
            AddLine(LightGray, $"You have {character.Satiety} Satiety").SetSfx(Sfx.Eat)
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
