open System.Windows.Forms
open System.Drawing

type MyForm() as this =
   inherit Form()
   do
    this.SetStyle(ControlStyles.DoubleBuffer, true)
    this.SetStyle(ControlStyles.UserPaint,true)
    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true)

let f = new MyForm(Text="What's the time?", TopMost=true)
f.Show()

let paint = ref (fun (g:Graphics) -> ())
f.Paint.Add(fun e -> !paint e.Graphics) 

let l = 20.

paint := fun g ->
    g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
    let pi = System.Math.PI
    let w = f.ClientSize.Width
    let h = f.ClientSize.Height
    // dichiaro la coppia di punti che rappresentano il centro del cerchio.
    let cx, cy = w / 2, h / 2
    // funzione che permette di convertire i punti in coordinate polari
    let convert(alpha, r) = 
        let a = alpha - (pi / 2.) // -pi/2 dipende dall'orientamento del piano cartesiano (x,-y)
        new PointF(single(cx) + single(r*(cos a)), single(cy) + single(r*(sin a)))
    let da = pi / 6.
    use dp = new Pen(Brushes.Black) // il costrutto use incarica il programmatore di rilasciare l'oggetto appena finito di usarlo
    dp.Width <- 2.f
    for a in 0. .. da .. (2. * pi + da) do
        let p1,p2 = convert(a,float(w/2) - l), convert(a,float(w/2))
        g.DrawLine(dp,p1,p2)
    
    let drawLancetta (p, a, l) = 
        let center = new PointF(single cx, single cy)
        let p1,p2 = convert (a + pi, 10.), convert(a, l)
        g.DrawLine(p, p1, p2)

    //Lancette
    let t = System.DateTime.Now
    let h = float(t.Hour % 12) * da + da * float(t.Minute) / 60. //anche le frazioni di ora
    let m = float(t.Minute) * pi / 30. // * 2pi/60
    let s = float(t.Second) * pi / 30.
    use ph = new Pen(Brushes.Black, Width=4.f)
    use pm = new Pen(Brushes.Blue, Width=2.f)
    use ps = new Pen(Brushes.Red)
    drawLancetta(ph, h, float(w/2) * 2./3.)
    drawLancetta(pm, m, float(w/2) * 3./4.)
    drawLancetta(ps, s, float(w/2) * 4./5.)

let t = new Timer(Interval=1000)
t.Tick.Add(fun _ -> f.Invalidate())
t.Start()

f.Width
f.Invalidate()

// Paint with double buffering
let buffer = ref(new Bitmap(f.Width, f.Height))

f.SizeChanged.Add(fun _ ->
    buffer := new Bitmap(f.ClientSize.Width,f.ClientSize.Height)
    f.Invalidate()
)