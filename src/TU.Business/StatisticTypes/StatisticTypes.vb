Friend Module StatisticTypes
    Friend Const Durability = "Durability"
    Friend ReadOnly FiberWeight As String = ForagingWeight(ItemTypes.Fiber)
    Friend Const Health = "Health"
    Friend Const HungerRate = "HungerRate"
    Friend Const MaximumAttack = "MaximumAttack"
    Friend Const MaximumDefend = "MaximumDefend"
    Friend Const MaximumDurability = "MaximumDurability"
    Friend Const MaximumHealth = "MaximumHealth"
    Friend Const MaximumSatiety = "MaximumSatiety"
    Friend Const MinimumAttack = "MinimumAttack"
    Friend Const MinimumDefend = "MinimumDefend"
    Friend ReadOnly MossWeight As String = ForagingWeight(ItemTypes.Moss)
    Friend Const MovesMade = "MovesMade"
    Friend Const Satiety = "Satiety"
    Friend Const ForageAttempts = "ForageAttempts"
    Friend ReadOnly Property ForagingWeight(itemType As String) As String
        Get
            Return $"{itemType}Weight"
        End Get
    End Property
End Module
