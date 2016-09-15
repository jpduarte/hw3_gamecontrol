/*
Surf Sim GUI code
*/


import processing.serial.*;                  // import the serial lin
float pitch, roll;                           // pitch and roll
float position;                              // position to translate to
int state = 0;
Serial myPort;                               // the serial port
PImage wave;
PFont start_text;
PFont play_text;
//states
//void (*next_state)() = start_screen;

void setup() {
  
  // draw the window:
  //size(500, 349, P3D);
  size(400,400,P3D);

  // calculate translate position for disc: 
  position = width/2;
  // List all the available serial ports 
  //println(Serial.list());
  // Open whatever port is the one you're using.
  myPort = new Serial(this, Serial.list()[2], 9600);
  // only generate a serial event when you get a newline: 
  myPort.bufferUntil('\n');
  wave = loadImage("wave400.jpg");
 
  // enable smoothing for 3D: 
  //hint(ENABLE_OPENGL_4X_SMOOTH);
}

void draw () {
  // colors inspired by the Amazon rainforest: 
  //background(#20542E);
  hint(DISABLE_DEPTH_TEST);
  background(wave);
  hint(ENABLE_DEPTH_TEST);
  
  //fill(#79BF3D);
  
  if(state == 0){
    //start_screen
    start_screen();
  }
  else if(state == 1){
  // draw the disc:
  tilt();
  }
  else if(state == 2){
  }
}

void start_screen() {
  start_text = createFont("chalkboard",100,true);
  textFont(start_text,36);
  fill(0xFFFF0000);
  textAlign(CENTER);
  text("Stand on Board to Begin!",width/2,height/2);
}

void tilt() {
  // translate from origin to center: 
  translate(position, position, position);
  //print(position);
  // X is front-to-back: 
  rotateX(radians(roll + 90)); 
  // Y is left-to-right: 
  rotateY(radians(pitch) );
  // set the disc fill color: 
  fill(#17C8D1);
  // draw the disc:
  ellipse(0, 0, width/4, width/4); 
  start_text = createFont("chalkboard",100,true);
  textFont(start_text,12);
  // set the text fill color: 
  fill(#20542E);
  // Draw some text so you can tell front from back:
  text(pitch + "," + roll, -40, 10, 1); 
}

void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
  myString = trim(myString);
  
  //print(myString,"\n");
  
  if(!myString.contains(",")){
    state = int(myString);
    print("State:",state,"\n");
  }
    
  //String state[] = split(\n
  // split the string at the commas 
  String items[] = split(myString, ','); 
  if (items.length > 1) {
      pitch = float(items[0]);
      roll = float(items[1]); 
      print("Pitch: ",pitch,"\tRoll: ",roll,"\n");
    }
  } 
}