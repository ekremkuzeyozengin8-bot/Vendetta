public class Main : Plugin<Config, Translation>
{
	private EventHandlers _eventHandlers;

	private CreditTag creditTag;

	private CoroutineHandle zoneCoroutine;

	public static Main Instance { get; private set; }

	public override string Name => "RPCommands";

	public override string Author => ".piwnica2137 & .Adamczyli";

	public override string Prefix => "RPCommands";

	public override Version Version => new Version(2, 1, 0);

	public override PluginPriority Priority => (PluginPriority)int.MinValue;

	public override void OnEnabled()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Instance = this;
		_eventHandlers = new EventHandlers();
		_eventHandlers.LoadEvents();
		_eventHandlers.RegisterCommands();
		creditTag = new CreditTag();
		creditTag.Load();
		zoneCoroutine = Timing.RunCoroutine(ZoneCommand.ZoneCoroutine());
		((Plugin<Config>)(object)this).OnEnabled();
	}

	public override void OnDisabled()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Instance = null;
		_eventHandlers.UnloadEvents();
		_eventHandlers = null;
		creditTag = null;
		Timing.KillCoroutines((CoroutineHandle[])(object)new CoroutineHandle[1] { zoneCoroutine });
		((Plugin<Config>)(object)this).OnDisabled();
	}
}

public class Config : IConfig
{
	[Description("true = Plugin enabled, false = plugin disabled")]
	public bool IsEnabled { get; set; } = true;

	[Description("Enable debug logs")]
	public bool Debug { get; set; } = false;

	[Description("Specifies how RP messages are displayed. Options: Hints, TextToys, Both")]
	public RPCommandsMode DisplayMode { get; set; } = RPCommandsMode.Both;

	[Description("The height at which the TextToy should appear above the player's head.")]
	public float TextToyHeightOffset { get; set; } = 1.1f;

	[Description("The font size for the TextToy.")]
	public float TextToySize { get; set; } = 3f;

	[Description("Enables or disables the in-game credit tag for the plugin's author.")]
	public bool IsCreditTagEnabled { get; set; } = true;

	[Description("List of banned words. Messages containing any of these words will be blocked. It is recommended to not delete 'size'")]
	public List<string> BannedWords { get; set; } = new List<string>(2) { "size", "<size>" };

	[Description("If false, SCPs will not be able to use RP Commands.")]
	public bool AllowScpToUseCommands { get; set; } = false;

	[Description("If true, sender will see a console message with the command they used if it's shown to others.")]
	public bool ShowCommandInSenderConsole { get; set; } = true;

	[Description("If true, spectators of players who are within range of the command will also see the hint.")]
	public bool ShowHintsToSpectatorsOfReceivers { get; set; } = true;

	[Description("Command settings, do not remove {0}, {1}, or {2}. For handler use: 'Client' for Player's Console, 'RemoteAdmin' for RA Text-Based.")]
	public CommandSettings Me { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Do { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Look { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Ooc { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Try { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!");

	public CommandSettings Desc { get; set; } = new CommandSettings(CommandHandlerType.Client, 15f, 5f, 3f, "<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Assist { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 0f, 3f, "<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}");

	[Description("Enable or disable sending notifications to a Discord webhook when the .assist command is used.")]
	public bool EnableAssistWebhook { get; set; } = false;

	[Description("The Discord webhook URL to send notifications to.")]
	public string AssistWebhookUrl { get; set; } = "https://discord.com/api/webhooks/your_webhook_url_here";

	public CommandSettings CustomInfo { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 0f, 0f, "");

	[Description("Maximum length of custom info")]
	public int MaxCustomInfoLength { get; set; } = 250;

	public CommandSettings Unwear { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	public CommandSettings Radio { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}");

	public CommandSettings Wear { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	[Description("Determines how the .wear command functions. Available options: RoleChange, ModelChange")]
	public WearMode WearMode { get; set; } = WearMode.RoleChange;

	[Description("Duration of the disguise from the .wear command in seconds. Set to -1 for infinite duration.")]
	public float WearDuration { get; set; } = 180f;

	public CommandSettings Punch { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	[Description("Damage dealt by the .punch command.")]
	public float PunchDamage { get; set; } = 5f;

	[Description("Push force multiplier for the .punch command.")]
	public float PunchPushForce { get; set; } = 50f;

	public CommandSettings Clean { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	public CommandSettings Heal { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	[Description("Amount of health restored by the .heal command.")]
	public float HealAmount { get; set; } = 65f;

	[Description("Item required to use the .heal command.")]
	public ItemType HealItem { get; set; } = (ItemType)14;

	public CommandSettings Cuff { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	[Description("Choose how cuffing affects a player's inventory. Options: SaveAndRestore, DropOnGround")]
	public CuffMode CuffBehavior { get; set; } = CuffMode.SaveAndRestore;

	[Description("Determines whether all SCPs can be cuffed.")]
	public bool CanCuffAllScps { get; set; } = false;

	[Description("A list of SCPs that are cuffable by default.")]
	public List<RoleTypeId> CuffableScps { get; set; } = new List<RoleTypeId>(1) { (RoleTypeId)5 };

	[Description("A list of items that can be used to cuff players.")]
	public List<ItemType> CuffingItems { get; set; } = new List<ItemType>(2)
	{
		(ItemType)20,
		(ItemType)24
	};

	public CommandSettings UnCuff { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 5f, 3f, "");

	public CommandSettings Name { get; set; } = new CommandSettings(CommandHandlerType.Client, 0f, 0f, 0f, "");

	[Description("If you want to increase size of the zone, set Range to higher value.")]
	public CommandSettings Zone { get; set; } = new CommandSettings(CommandHandlerType.Client, 5f, 30f, 10f, "<color=green>「Zone」</color><color=#FFFF00>{0}</color> : {1}");

	[Description("Enable or disable specific commands")]
	public Dictionary<string, bool> EnabledCommands { get; set; } = new Dictionary<string, bool>
	{
		{ "me", true },
		{ "do", true },
		{ "look", true },
		{ "ooc", true },
		{ "try", true },
		{ "desc", true },
		{ "custom-info", true },
		{ "assist", true },
		{ "radio", true },
		{ "wear", true },
		{ "punch", true },
		{ "clean", true },
		{ "heal", true },
		{ "cuff", true },
		{ "uncuff", true },
		{ "name", true },
		{ "zone", true },
		{ "unwear", true }
	};

	public bool IsCommandEnabled(string command)
	{
		return EnabledCommands.ContainsKey(command) && EnabledCommands[command];
	}

	public float GetRange(string command)
	{
		return GetSettings(command).Range;
	}

	public float GetDuration(string command)
	{
		return GetSettings(command).Duration;
	}

	public float GetCooldown(string command)
	{
		return GetSettings(command).Cooldown;
	}

	public string FormatMessage(string command, params object[] args)
	{
		return string.Format(GetSettings(command).Format, args);
	}

	public CommandSettings GetSettings(string command)
	{
		if (1 == 0)
		{
		}
		CommandSettings result = command switch
		{
			"me" => Me, 
			"do" => Do, 
			"look" => Look, 
			"ooc" => Ooc, 
			"try" => Try, 
			"desc" => Desc, 
			"assist" => Assist, 
			"custom-info" => CustomInfo, 
			"radio" => Radio, 
			"wear" => Wear, 
			"punch" => Punch, 
			"clean" => Clean, 
			"heal" => Heal, 
			"cuff" => Cuff, 
			"uncuff" => UnCuff, 
			"name" => Name, 
			"zone" => Zone, 
			"unwear" => Unwear, 
			_ => throw new ArgumentException("Invalid command", "command"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}

public abstract class RPCommand : ICommand
{
	private static readonly Dictionary<Player, Dictionary<string, float>> PlayerCooldowns = new Dictionary<Player, Dictionary<string, float>>();

	public abstract string OriginalCommand { get; }

	public virtual string[] Aliases => Array.Empty<string>();

	public abstract string Description { get; }

	public virtual bool AllowNoArguments => false;

	public string Command
	{
		get
		{
			string value;
			return ((Plugin<Config, Translation>)Main.Instance).Translation.CommandNames.TryGetValue(OriginalCommand, out value) ? value : OriginalCommand;
		}
	}

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		PlayerCommandSender val = (PlayerCommandSender)(object)((sender is PlayerCommandSender) ? sender : null);
		if (val == null)
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.OnlyPlayers;
			return false;
		}
		Player val2 = Player.Get(val.ReferenceHub);
		if (!((Plugin<Config>)(object)Main.Instance).Config.IsCommandEnabled(OriginalCommand))
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.CommandDisabled;
			return false;
		}
		if (!Round.IsStarted && OriginalCommand != "assist")
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.RoundNotStarted;
			return false;
		}
		if (val2.IsScp && !((Plugin<Config>)(object)Main.Instance).Config.AllowScpToUseCommands)
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.OnlyHumans;
			return false;
		}
		if (!val2.IsAlive && OriginalCommand != "assist")
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.OnlyAlive;
			return false;
		}
		if (arguments.Count < 1 && !AllowNoArguments)
		{
			response = string.Format(((Plugin<Config, Translation>)Main.Instance).Translation.Usage, Command);
			return false;
		}
		if (HasCooldown(val2, out var remainingTime))
		{
			if (((Plugin<Config, Translation>)Main.Instance).Translation.CommandCooldown.Contains("{0}"))
			{
				response = string.Format(((Plugin<Config, Translation>)Main.Instance).Translation.CommandCooldown, Math.Ceiling(remainingTime));
			}
			else
			{
				response = ((Plugin<Config, Translation>)Main.Instance).Translation.CommandCooldown;
			}
			return false;
		}
		string message = string.Join(" ", arguments);
		if (((Plugin<Config>)(object)Main.Instance).Config.BannedWords.Any((string bannedWord) => message.ToLower().Contains(bannedWord.ToLower())))
		{
			response = ((Plugin<Config, Translation>)Main.Instance).Translation.BannedWordDetected;
			return false;
		}
		if (!ExecuteAction(val2, message, out response))
		{
			return false;
		}
		SetCooldown(val2);
		Request.SetLastMessage(val2, OriginalCommand, message);
		return true;
	}

	protected virtual bool ExecuteAction(Player player, string message, out string response)
	{
		float range = ((Plugin<Config>)(object)Main.Instance).Config.GetRange(OriginalCommand);
		float duration = ((Plugin<Config>)(object)Main.Instance).Config.GetDuration(OriginalCommand);
		string message2 = FormatMessage(player, message);
		DisplayMessage(player, message2, range, duration);
		response = ((Plugin<Config, Translation>)Main.Instance).Translation.MessageSent;
		return true;
	}

	private void DisplayMessage(Player sender, string message, float range, float duration)
	{
		switch (((Plugin<Config>)(object)Main.Instance).Config.DisplayMode)
		{
		case RPCommandsMode.Hints:
			HintToNearbyPlayers(sender, message, range, duration);
			break;
		case RPCommandsMode.TextToys:
			SpawnTextToyForSender(sender, message, duration, showInConsole: true);
			if (((Plugin<Config>)(object)Main.Instance).Config.ShowHintsToSpectatorsOfReceivers)
			{
				SendHintToSpectatorsOfNearbyPlayers(sender, message, range, duration);
			}
			break;
		case RPCommandsMode.Both:
			HintToNearbyPlayers(sender, message, range, duration);
			SpawnTextToyForSender(sender, message, duration, showInConsole: false);
			break;
		}
	}

	private void SpawnTextToyForSender(Player sender, string message, float duration, bool showInConsole)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (showInConsole && ((Plugin<Config>)(object)Main.Instance).Config.ShowCommandInSenderConsole)
			{
				foreach (Player item in Player.List.Where((Player p) => Vector3.Distance(p.Position, sender.Position) <= ((Plugin<Config>)(object)Main.Instance).Config.GetRange(OriginalCommand)))
				{
					item.SendConsoleMessage(message ?? "", "yellow");
				}
			}
			TextToy prefab = Text.Prefab;
			if ((Object)(object)prefab == (Object)null)
			{
				Log.Error("TextToy prefab is null. Cannot spawn TextToy.");
				return;
			}
			TextToy val = Object.Instantiate<TextToy>(prefab);
			((Component)val).transform.position = sender.Position + Vector3.up * ((Plugin<Config>)(object)Main.Instance).Config.TextToyHeightOffset;
			((Component)val).transform.rotation = sender.Transform.rotation;
			((Component)val).transform.localScale = Vector3.one;
			NetworkServer.Spawn(((Component)val).gameObject, (NetworkConnection)null);
			val.TextFormat = $"<size={((Plugin<Config>)(object)Main.Instance).Config.TextToySize}>{message}</size>";
			TextToy controller = ((Component)val).gameObject.AddComponent<TextToy>();
			controller.Initialize(sender, val, ((Plugin<Config>)(object)Main.Instance).Config.TextToyHeightOffset);
			sender.Connection.Send<ObjectDestroyMessage>(new ObjectDestroyMessage
			{
				netId = ((NetworkBehaviour)val).netId
			}, 0);
			Timing.CallDelayed(duration, (Action)delegate
			{
				controller?.DestroyToy();
			});
		}
		catch (Exception arg)
		{
			Log.Error($"Failed to spawn TextToy: {arg}");
		}
	}

	protected virtual string FormatMessage(Player player, string message)
	{
		return ((Plugin<Config>)(object)Main.Instance).Config.FormatMessage(OriginalCommand, player.Nickname, message);
	}

	private bool HasCooldown(Player player, out float remainingTime)
	{
		if (PlayerCooldowns.TryGetValue(player, out var value) && value.TryGetValue(OriginalCommand, out var value2))
		{
			remainingTime = value2 - Time.time;
			return remainingTime > 0f;
		}
		remainingTime = 0f;
		return false;
	}

	private void SetCooldown(Player player)
	{
		if (!PlayerCooldowns.ContainsKey(player))
		{
			PlayerCooldowns[player] = new Dictionary<string, float>();
		}
		float cooldown = ((Plugin<Config>)(object)Main.Instance).Config.GetCooldown(OriginalCommand);
		PlayerCooldowns[player][OriginalCommand] = Time.time + cooldown;
	}

	private void HintToNearbyPlayers(Player sender, string message, float range, float duration)
	{
		bool showCommandInSenderConsole = ((Plugin<Config>)(object)Main.Instance).Config.ShowCommandInSenderConsole;
		foreach (Player item in Player.List.Where((Player p) => p != sender && Vector3.Distance(p.Position, sender.Position) <= range))
		{
			SendHint(item, message, duration);
			if (showCommandInSenderConsole)
			{
				item.SendConsoleMessage(message ?? "", "yellow");
			}
		}
		SendHint(sender, message, duration);
		if (showCommandInSenderConsole)
		{
			sender.SendConsoleMessage(message ?? "", "yellow");
		}
	}

	private void SendHintToSpectatorsOfNearbyPlayers(Player sender, string message, float range, float duration)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Player> hashSet = new HashSet<Player>();
		foreach (Player item in Player.List.Where((Player p) => Vector3.Distance(p.Position, sender.Position) <= range))
		{
			foreach (Player currentSpectatingPlayer in item.CurrentSpectatingPlayers)
			{
				if (currentSpectatingPlayer.IsConnected)
				{
					hashSet.Add(currentSpectatingPlayer);
				}
			}
		}
		foreach (Player item2 in hashSet)
		{
			DynamicHint hint = new DynamicHint
			{
				Text = message,
				TargetY = 800f,
				TargetX = -950f,
				FontSize = 25
			};
			PlayerDisplay observerDisplay = PlayerDisplay.Get(item2);
			PlayerDisplay obj = observerDisplay;
			if (obj != null)
			{
				obj.AddHint((AbstractHint)(object)hint);
			}
			Timing.CallDelayed(duration, (Action)delegate
			{
				PlayerDisplay obj2 = observerDisplay;
				if (obj2 != null)
				{
					obj2.RemoveHint((AbstractHint)(object)hint);
				}
			});
		}
	}

	public void SendHint(Player player, string message, float duration)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		DynamicHint hint = new DynamicHint
		{
			Text = message,
			TargetY = 760f,
			TargetX = -950f,
			FontSize = 25
		};
		PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
		PlayerDisplay obj = playerDisplay;
		if (obj != null)
		{
			obj.AddHint((AbstractHint)(object)hint);
		}
		Timing.CallDelayed(duration, (Action)delegate
		{
			PlayerDisplay obj3 = playerDisplay;
			if (obj3 != null)
			{
				obj3.RemoveHint((AbstractHint)(object)hint);
			}
		});
		if (!((Plugin<Config>)(object)Main.Instance).Config.ShowHintsToSpectatorsOfReceivers)
		{
			return;
		}
		foreach (Player currentSpectatingPlayer in player.CurrentSpectatingPlayers)
		{
			if (!currentSpectatingPlayer.IsConnected)
			{
				continue;
			}
			PlayerDisplay observerDisplay = PlayerDisplay.Get(currentSpectatingPlayer);
			PlayerDisplay obj2 = observerDisplay;
			if (obj2 != null)
			{
				obj2.AddHint((AbstractHint)(object)hint);
			}
			Timing.CallDelayed(duration, (Action)delegate
			{
				PlayerDisplay obj3 = observerDisplay;
				if (obj3 != null)
				{
					obj3.RemoveHint((AbstractHint)(object)hint);
				}
			});
		}
	}
}
