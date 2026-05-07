namespace BAIO.Core.UnitData;

public class AttackAnimationData
{
    public AttackAnimationData(
        string unitName,
        string unitNetworkName,
        double attackRate,
        double attackPoint,
        double attackBackswing,
        int projectileSpeed,
        double turnRate)
    {
        this.UnitName = unitName;
        this.UnitNetworkName = unitNetworkName;
        this.AttackRate = attackRate;
        this.AttackPoint = attackPoint;
        this.AttackBackswing = attackBackswing;
        this.ProjectileSpeed = projectileSpeed;
        this.TurnRate = turnRate;
    }

    public AttackAnimationData()
    {
        // If this comment is removed the program will blow up.
    }

    public double AttackBackswing { get; set; }

    /// <summary>
    ///     The attack animation point.
    /// </summary>
    public double AttackPoint { get; set; }

    /// <summary>
    ///     The attack animation rate.
    /// </summary>
    public double AttackRate { get; set; }

    /// <summary>
    ///     The attack animation projectile speed.
    /// </summary>
    public int ProjectileSpeed { get; set; }

    /// <summary>
    ///     The attack animation turn rate.
    /// </summary>
    public double TurnRate { get; set; }

    /// <summary>
    ///     The unit class id.
    /// </summary>
    public string UnitNetworkName { get; set; }

    /// <summary>
    ///     The unit name.
    /// </summary>
    public string UnitName { get; set; }
}