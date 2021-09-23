# QM project README

### Classes:

---

Program - Main program; contains user input loop

ConsoleApp - Functions to handle user input and send to StageControls

StageControls - Sends ASCII commands to driver

BasicConnection - Serial port connection, communication with driver

todo: 

AlignerContorls - Sends commands to aligner

### Description:

---

Currently this program is to test the drivers and motors, but I'm hoping some classes can be used in the final UI. I'm making the StageControl  and AlignerContorls classes so that they can be used with any UI. Please advice how I can do this better.

### Async serial port:

---

BasicConnection does not use asyn serial port reading. In my use case it's unnecessary because reading is fast and most operations require to wait for response anyway.

But I have been thinking about how to make async and reading this: [https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport](https://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport).

### Async Waiting for move to complete

Currently when a move happens Wait() is run to wait for the move to complete. I'm try to figure out the best way to make this async. The axis in motion should be blocked so a second move cannot start before first one is complete.
