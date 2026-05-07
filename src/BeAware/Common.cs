namespace BeAware;

using System;

using BeAware.Entities;
using BeAware.Helpers;
using BeAware.MenuManager;
using BeAware.Overlay;
using BeAware.ShowMeMore;
using BeAware.ShowMeMore.MoreInformation;

using Range = ShowMeMore.Range;

internal class Common : IDisposable
{
    public MenuConfig MenuConfig { get; }

    public MessageCreator MessageCreator { get; }

    public SoundHelper SoundHelper { get; }

    //private AllyOverlay AllyOverlay { get; }

    //private EnemyOverlay EnemyOverlay { get; }

    //private NetworthPanel NetworthPanel { get; }

    public Verification Verification { get; }

    public SpiritBreakerCharge SpiritBreakerCharge { get; }

    public MiranaArrow MiranaArrow { get; }

    public InvokerEMP InvokerEMP { get; }

    public InvokerSunStrike InvokerSunStrike { get; }

    public KunkkaTorrent KunkkaTorrent { get; }

    public LeshracSplitEarth LeshracSplitEarth { get; }

    public LifeStealerInfest LifeStealerInfest { get; }

    public LinaLightStrikeArray LinaLightStrikeArray { get; }

    public PhantomAssassinBlur PhantomAssassinBlur { get; }

    public PudgeHook PudgeHook { get; }

    public AncientApparitionIceBlast AncientApparitionIceBlast { get; }

    public BloodseekerRupture BloodseekerRupture { get; }

    public WindrunnerPowershot WindrunnerPowershot { get; }

    //public IllusionShow IllusionShow { get; }

    //public LinkenShow LinkenShow { get; }

    public Additional Additional { get; }

    //private TowerHelper TowerHelper { get; }

    //private readonly VisibleByEnemy VisibleByEnemy;

    //private readonly TrueSightVision TrueSightVision;

    private readonly Range Range;

    private ParticleMonitor ParticleMonitor { get; }

    private UnitMonitor UnitMonitor { get; }

    private ModifierMonitor ModifierMonitor { get; }

    private bool Disposed { get; set; }

    public Common(MenuConfig menuConfig)
    {
        MenuConfig = menuConfig;

        MessageCreator = new MessageCreator(this);
        SoundHelper = new SoundHelper(this);

        //AllyOverlay = new AllyOverlay(this);
        //EnemyOverlay = new EnemyOverlay(this);
        //NetworthPanel = new NetworthPanel(this);

        Verification = new Verification(this);

        SpiritBreakerCharge = new SpiritBreakerCharge(this);
        MiranaArrow = new MiranaArrow(this);
        InvokerEMP = new InvokerEMP(this);
        InvokerSunStrike = new InvokerSunStrike(this);
        KunkkaTorrent = new KunkkaTorrent(this);
        LeshracSplitEarth = new LeshracSplitEarth(this);
        LifeStealerInfest = new LifeStealerInfest(this);
        LinaLightStrikeArray = new LinaLightStrikeArray(this);
        PhantomAssassinBlur = new PhantomAssassinBlur(this);
        PudgeHook = new PudgeHook(this);
        AncientApparitionIceBlast = new AncientApparitionIceBlast(this);
        BloodseekerRupture = new BloodseekerRupture(this);
        WindrunnerPowershot = new WindrunnerPowershot(this);

        //IllusionShow = new IllusionShow(this);
        //LinkenShow = new LinkenShow(this);
        Additional = new Additional(this);
        //TowerHelper = new TowerHelper(this);
        //VisibleByEnemy = new VisibleByEnemy(this);
        //TrueSightVision = new TrueSightVision(this);
        Range = new Range(this);

        ParticleMonitor = new ParticleMonitor(this);
        UnitMonitor = new UnitMonitor(this);
        ModifierMonitor = new ModifierMonitor(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
        {
            return;
        }

        if (disposing)
        {
            ModifierMonitor.Dispose();
            UnitMonitor.Dispose();
            ParticleMonitor.Dispose();

            SpiritBreakerCharge.Dispose();

            Range.Dispose();
            //TrueSightVision.Dispose();
            //VisibleByEnemy.Dispose();
            //TowerHelper.Dispose();
            Additional.Dispose();
            //LinkenShow.Dispose();
            //IllusionShow.Dispose();

            Verification.Dispose();

            //NetworthPanel.Dispose();
            //EnemyOverlay.Dispose();
            //AllyOverlay.Dispose();
        }

        Disposed = true;
    }
}