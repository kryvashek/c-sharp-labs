
// This file has been generated by the GUI designer. Do not modify.
namespace lab07
{
	public partial class MainWindow
	{
		private global::Gtk.UIManager UIManager;

		private global::Gtk.Action openAction;

		private global::Gtk.Action saveAsAction;

		private global::Gtk.Action convertAction;

		private global::Gtk.Action indexAction;

		private global::Gtk.Action quitAction;

		private global::Gtk.VPaned vpaned;

		private global::Gtk.VBox vboxUp;

		private global::Gtk.Toolbar toolBar;

		private global::Gtk.DrawingArea drawingArea;

		private global::Gtk.VBox vboxDown;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TextView viewLogText;

		private global::Gtk.Statusbar statusBar;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget lab07.MainWindow
			this.UIManager = new global::Gtk.UIManager();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup("Default");
			this.openAction = new global::Gtk.Action("openAction", global::Mono.Unix.Catalog.GetString("Открыть"), null, "gtk-open");
			this.openAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Открыть");
			w1.Add(this.openAction, "<Primary><Mod2>o");
			this.saveAsAction = new global::Gtk.Action("saveAsAction", global::Mono.Unix.Catalog.GetString("Сохранить схему"), null, "gtk-save-as");
			this.saveAsAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Сохранить схему");
			w1.Add(this.saveAsAction, "<Primary><Mod2>s");
			this.convertAction = new global::Gtk.Action("convertAction", global::Mono.Unix.Catalog.GetString("Сохранить картинку"), null, "gtk-convert");
			this.convertAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Сохранить картинку");
			w1.Add(this.convertAction, "<Primary><Alt><Mod2>s");
			this.indexAction = new global::Gtk.Action("indexAction", global::Mono.Unix.Catalog.GetString("Сохранить журнал"), null, "gtk-index");
			this.indexAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Сохранить журнал");
			w1.Add(this.indexAction, "<Alt><Mod2>s");
			this.quitAction = new global::Gtk.Action("quitAction", global::Mono.Unix.Catalog.GetString("Выйти"), null, "gtk-quit");
			this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString("Выйти");
			w1.Add(this.quitAction, "<Primary><Mod2>q");
			this.UIManager.InsertActionGroup(w1, 0);
			this.AddAccelGroup(this.UIManager.AccelGroup);
			this.Name = "lab07.MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child lab07.MainWindow.Gtk.Container+ContainerChild
			this.vpaned = new global::Gtk.VPaned();
			this.vpaned.CanFocus = true;
			this.vpaned.Name = "vpaned";
			this.vpaned.Position = 320;
			// Container child vpaned.Gtk.Paned+PanedChild
			this.vboxUp = new global::Gtk.VBox();
			this.vboxUp.Name = "vboxUp";
			this.vboxUp.Spacing = 6;
			// Container child vboxUp.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString(@"<ui><toolbar name='toolBar'><toolitem name='openAction' action='openAction'/><toolitem name='saveAsAction' action='saveAsAction'/><toolitem name='convertAction' action='convertAction'/><toolitem name='indexAction' action='indexAction'/><toolitem name='quitAction' action='quitAction'/></toolbar></ui>");
			this.toolBar = ((global::Gtk.Toolbar)(this.UIManager.GetWidget("/toolBar")));
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowArrow = false;
			this.toolBar.ToolbarStyle = ((global::Gtk.ToolbarStyle)(2));
			this.toolBar.IconSize = ((global::Gtk.IconSize)(1));
			this.vboxUp.Add(this.toolBar);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxUp[this.toolBar]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vboxUp.Gtk.Box+BoxChild
			this.drawingArea = new global::Gtk.DrawingArea();
			this.drawingArea.Name = "drawingArea";
			this.vboxUp.Add(this.drawingArea);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vboxUp[this.drawingArea]));
			w3.Position = 1;
			this.vpaned.Add(this.vboxUp);
			global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.vpaned[this.vboxUp]));
			w4.Resize = false;
			// Container child vpaned.Gtk.Paned+PanedChild
			this.vboxDown = new global::Gtk.VBox();
			this.vboxDown.Name = "vboxDown";
			this.vboxDown.Spacing = 6;
			// Container child vboxDown.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.viewLogText = new global::Gtk.TextView();
			this.viewLogText.CanFocus = true;
			this.viewLogText.Name = "viewLogText";
			this.GtkScrolledWindow.Add(this.viewLogText);
			this.vboxDown.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vboxDown[this.GtkScrolledWindow]));
			w6.Position = 0;
			// Container child vboxDown.Gtk.Box+BoxChild
			this.statusBar = new global::Gtk.Statusbar();
			this.statusBar.Name = "statusBar";
			this.statusBar.Spacing = 6;
			this.vboxDown.Add(this.statusBar);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxDown[this.statusBar]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			this.vpaned.Add(this.vboxDown);
			this.Add(this.vpaned);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 697;
			this.DefaultHeight = 418;
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.effectDelete);
			this.openAction.Activated += new global::System.EventHandler(this.effectLoadScheme);
			this.saveAsAction.Activated += new global::System.EventHandler(this.effectSaveScheme);
			this.convertAction.Activated += new global::System.EventHandler(this.effectSavePic);
			this.indexAction.Activated += new global::System.EventHandler(this.effectSaveLog);
			this.quitAction.Activated += new global::System.EventHandler(this.effectQuit);
			this.drawingArea.MotionNotifyEvent += new global::Gtk.MotionNotifyEventHandler(this.effectMotion);
			this.drawingArea.ExposeEvent += new global::Gtk.ExposeEventHandler(this.effectExpose);
		}
	}
}
