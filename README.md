# QM project README

## Classes:

Main - Intializes connections and controllers, opens ports, being user input loop

UserInputHandler - Gets user input string, modifies, and sends to contorollers

StageController - Sends ASCII commands to move X,Y,Z,T

AlignerController - Sends ASCII commands to move aligner

Connection - Serial port connection, read/writes commands, Handles Command Queue


## Description:

Program to control X,Y,Z,T stages and aligner. 

## Commands:

End and Exit: quit

Reset all Amplifiers: reset

Home an axis: home <axis>

Move by Absolute: moveabs <axis> <value>

Move by Relative: moverel <axis> <value>

Get Position: pos <axis>

Get/Set velocity: vel <axis> <value>

Get/Set move distance: dis <axis> <value>

Get/Set acceleration: accel <axis> <value>

Get/Set deceleration: decel <axis> <value>

Activate Joystick Fast mode: joyfast <axis>

Activate Joystick slow mode: joyfast <axis>

Turn off joystick: joyoff <axis>

Read Error codes: error <axis>

Turn on motor: on <axis>
  
Turn off motor: off <axis>

Run routine Align 300mm: alignwafer300
  
Run routine Align 200mm: alignwafer200
  
Run routine Align 150mm: alignwafer150
  
Run routine go to wafer pick position: pickupwafer

