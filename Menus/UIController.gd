extends CanvasLayer

# References to UI elements
@onready var background = $Background
@onready var title_label = $Background/OverlayPanel/TitleLabel
@onready var subtitle_label = $Background/OverlayPanel/SubtitleLabel
@onready var resume_button = $Background/OverlayPanel/VBoxContainer/ResumeButton
@onready var restart_button = $Background/OverlayPanel/VBoxContainer/RestartButton
@onready var quit_button = $Background/OverlayPanel/VBoxContainer/QuitButton

# State tracking
var is_paused = false
var current_state = "none"  # none, pause, end
var end_state = null  # Will store "victory" or "defeat"
@export var menu_toggle_sfx: AudioStream
var menu_sfx_player: AudioStreamPlayer

func _ready():
	# Connect button signals
	resume_button.pressed.connect(_on_resume_pressed)
	restart_button.pressed.connect(_on_restart_pressed)
	quit_button.pressed.connect(_on_quit_pressed)
	menu_sfx_player = AudioStreamPlayer.new()
	menu_sfx_player.name = "MenuSfxPlayer"
	add_child(menu_sfx_player)
	
	# Set to always process even when tree is paused
	process_mode = Node.PROCESS_MODE_ALWAYS
	
	# Connect to World signals
	var world = get_tree().root.get_node("World")
	
	var error1 = world.connect("PauseGame", Callable(self, "_on_pause_requested"))
	if error1 != OK:
		push_warning("Failed to connect PauseGame signal.")
	
	var error2 = world.connect("GameOver", Callable(self, "_on_game_over"))
	if error2 != OK:
		push_warning("Failed to connect GameOver signal.")

func _process(_delta):
	# Allow ESC key to toggle pause menu open/closed
	if Input.is_action_just_pressed("ui_cancel") and (current_state == "none" or current_state == "pause"):
		_toggle_pause()

func _toggle_pause():
	if is_paused:
		_resume_game()
	else:
		_show_pause_menu()

func _show_pause_menu():
	is_paused = true
	current_state = "pause"
	get_tree().paused = true
	_play_menu_toggle_sfx()
	
	title_label.text = "PAUSED"
	subtitle_label.text = ""
	resume_button.text = "Resume"
	resume_button.visible = true
	background.visible = true

func _resume_game():
	is_paused = false
	current_state = "none"
	get_tree().paused = false
	background.visible = false
	_play_menu_toggle_sfx()

func _on_pause_requested():
	_toggle_pause()

func _on_game_over(end_state_value: int):
	# end_state_value: 0 = Defeat, 1 = Victory
	is_paused = true
	current_state = "end"
	get_tree().paused = true
	_play_menu_toggle_sfx()
	
	if end_state_value == 0:  # Defeat
		end_state = "defeat"
		title_label.text = "DEFEAT"
		subtitle_label.text = "The Earth has been conquered.\nGame Over."
	else:  # Victory
		end_state = "victory"
		title_label.text = "VICTORY"
		subtitle_label.text = "Congratulations!\nYou have survived all attacks!"
	
	resume_button.visible = false
	background.visible = true
	
	print("Game over screen: ", end_state)

func _on_resume_pressed():
	if current_state == "pause":
		_resume_game()
	elif current_state == "end":
		# Close the end screen but stay paused
		background.visible = false
		current_state = "none"
		is_paused = false
		get_tree().paused = false

func _on_restart_pressed():
	get_tree().paused = false
	get_tree().reload_current_scene()

func _on_quit_pressed():
	get_tree().paused = false
	get_tree().quit()

func _play_menu_toggle_sfx():
	if menu_toggle_sfx == null or menu_sfx_player == null:
		return

	menu_sfx_player.stream = menu_toggle_sfx
	menu_sfx_player.play()
