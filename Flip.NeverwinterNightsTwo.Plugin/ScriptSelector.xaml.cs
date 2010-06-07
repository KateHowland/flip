using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Sussex.Flip.Games.NeverwinterNightsTwo.Utils;
using Sussex.Flip.UI;

namespace Sussex.Flip.Games.NeverwinterNightsTwo
{
    /// <summary>
    /// Interaction logic for ScriptSelector.xaml
    /// </summary>
    public partial class ScriptSelector : Window
    {
    	protected TriggerControl selected;
    	
    	
		public TriggerControl Selected {
			get { return selected; }
		}
    	
    	
    	public ScriptSelector()
        {
    		selected = null;
    		
    		List<TriggerControl> triggers = GetDefaultTriggersFromAddresses();
            InitializeComponent();
            
            MouseButtonEventHandler scriptSelected = new MouseButtonEventHandler(OpenScript);
            scriptsListBox.MouseDoubleClick += scriptSelected;
            
            Thickness thickness = new Thickness(3);
            
            foreach (TriggerControl trigger in triggers) {
            	trigger.LayoutTransform = new System.Windows.Media.ScaleTransform(0.7,0.7);
            	trigger.Padding = thickness;
            	scriptsListBox.Items.Add(trigger);
            }
        }
    	
    	
    	protected List<TriggerControl> GetDefaultTriggersFromAddresses()
    	{
        	Fitter fitter = new AnyFitter();
        	
        	List<TriggerControl> triggers = new List<TriggerControl>(4);
        	
        	Nwn2TriggerFactory factory = new Nwn2TriggerFactory(new Nwn2Fitters());
        	
        	Nwn2AddressFactory aFactory = new Nwn2AddressFactory();
        	
        	List<Nwn2Address> addresses = new List<Nwn2Address>(3);
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	addresses.Add(aFactory.GetAreaAddress("OnClientEnterScript","area1"));
        	addresses.Add(aFactory.GetModuleAddress("OnPlayerRespawn"));
        	addresses.Add(aFactory.GetInstanceAddress("OnDeath","area1",Nwn2Type.Creature,"creature1"));
        	
        	foreach (Nwn2Address address in addresses) {
        		triggers.Add(factory.GetTriggerFromAddress(address));
        	}
        	
        	return triggers;
    	}
    	
    	
    	protected List<TriggerControl> GetDefaultTriggers()
    	{        	
        	Fitter fitter = new AnyFitter();
        	Nwn2ObjectBlockFactory blocks = new Nwn2ObjectBlockFactory(new Nwn2ImageProvider(new Sussex.Flip.Games.NeverwinterNightsTwo.Integration.NarrativeThreadsHelper()));
        	
        	AreaEntered entered = new AreaEntered(fitter);
        	entered.RaiserBlock = blocks.CreateAreaBlock(new Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.AreaBehaviour("forest1","Evil Forest",true));
        	
        	AreaEntered entered2 = new AreaEntered(fitter);
        	entered2.RaiserBlock = blocks.CreateAreaBlock(new Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.AreaBehaviour("dung1","Wizard Dungeon",false));
        	
        	CreatureDies dies = new CreatureDies(fitter);
        	dies.RaiserBlock = blocks.CreateInstanceBlock(new Sussex.Flip.Games.NeverwinterNightsTwo.Behaviours.InstanceBehaviour("wlf1",
        	                                                                                                                      "Wild Wolf",
        	                                                                                                                      Sussex.Flip.Games.NeverwinterNightsTwo.Utils.Nwn2Type.Creature,
        	                                                                                                                      "forest1",
        	                                                                                                                      "c_dogwolf",
        	                                                                                                                      string.Empty));
        	
        	ModuleHeartbeat heartbeat = new ModuleHeartbeat();
        	
        	List<TriggerControl> triggers = new List<TriggerControl>(4);
        	triggers.Add(entered);
        	triggers.Add(entered2);
        	triggers.Add(dies);
        	triggers.Add(heartbeat);
        	return triggers;
    	}

    	
        protected void OpenScript(object sender, EventArgs e)
        {
        	OpenSelectedScript();
        }
        
        
        protected void OpenSelectedScript()
        {
        	selected = scriptsListBox.SelectedItem as TriggerControl;
        	
        	if (selected == null) {
        		MessageBox.Show("Select a script to open, or click Cancel.");
        	}
        	else {
        		Close();
        	}
        }
        
        
        protected void CancelOpen(object sender, EventArgs e)
        {
        	Close();
        }
    }
}