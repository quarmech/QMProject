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

End and Exit: **quit**

Reset all Amplifiers: **reset**

<ins>Stage Commands:</ins>

Read Error codes: **error \<axis>**

stop all motion: **stop**

Home an axis: **home \<axis>**

Move by Absolute: **moveabs \<axis> \<value>**

Move by Relative: **moverel \<axis> \<value>**

Get Position: **pos \<axis>**

Get/Set velocity: **vel \<axis> \<value>**

Get/Set move distance: **dis \<axis> \<value>**

Get/Set acceleration: **accel \<axis> \<value>**

Get/Set deceleration: **decel \<axis> \<value>**

move to center position: **center \<axis>**

move to positive limit position: **poslimit \<axis>**

Turn on motor: **on \<axis>**
  
Turn off motor: **off \<axis>**

<ins>joystick:</ins>

Activate Joystick Fast mode: **joyfast \<axis>**

Activate Joystick slow mode: **joyfast \<axis>**

Turn off joystick: **joyoff \<axis>**

Activate fast mode all axis: **joyonall**

deactivate all axis: **joyoffall**

joystick status: **joystatus**

<ins>solenoid valves:</ins>

Turn on Festo solenoid: **fsol \<number> on**

Turn off Festo solenoid: **fsol \<number> off**

grip: **grip**

ungrip: **ungrip**

toggle tilt break: **tbreak**

get tilt break status: **tbreakstatus**

<ins>routines:</ins>

Run routine Align 300mm: **alignwafer300**
  
Run routine Align 200mm: **alignwafer200**
  
Run routine Align 150mm: **alignwafer150**
  
Run routine go to wafer pick position: **pickupwafer**

<ins>vacuum:</ins>

turn on vacuum: **vacuumOn**

turn on vacuum: **vacuumOff**

vacuum status: **vacuumstatus**

<ins>aligner commands</ins>

All aligner commands are in the document: Commands.txt
To use those commands type "a" in front.

raise chuck up: **a ZMU**

raise chuck up with vacuum: **a ZVMU**

Lower chuck: **a ZMD**

find notch or flat: **a APF**

<ins>other aligner commands:</ins>

rotate wafer to absolute degree: **rotatewafer \<degree>**






