PGraphics target;
PGraphics board;

import processing.serial.*;                  // import the serial lin
float pitch, roll;                           // pitch and roll
float target_pitch, target_roll;
float new_target_pitch, new_target_roll;
float positionx, positiony;                              // position to translate to
int state = 0;
Serial myPort;                               // the serial port
PImage wave;
PFont start_text;
PFont nailed_it_text;
PFont play_text;
boolean new_target = true;
boolean holding = false;
boolean nailed_it = false;
float eps = 20;
float tintr = 255, tintb = 255, tintg = 255;
PShape SurfBoard;
int hold = 2000;
int hold_time, time;
boolean moving_roll = true, moving_pitch = true;
String[] nailed_it_strings = {"Nailed It!","Radical!","Get Pitted!","Gnarly!","Killing It!"};
int num_string = 5;
int i = 0;
boolean printed_text = false;

void setup() {
  size(736, 552, P3D); 
  target = createGraphics(width, height, P3D);
  board = createGraphics(width, height, P3D);
   wave = loadImage("wave_2.jpg");
   
    // myPort = new Serial(this, Serial.list()[0], 9600);
  // only generate a serial event when you get a newline: 
    //myPort.bufferUntil('\n');
    //SurfBoard = loadShape("SurfBoard.obj");
} 

void draw() {
  //background(#20542E);
  hint(DISABLE_DEPTH_TEST);
  background(wave);
  hint(ENABLE_DEPTH_TEST);  
  
  if(state == 0) {
    start_screen();
  }
  else if(state == 1) {
    play();
  }
  else if(state == 2) {
  }

}

void start_screen() {
  start_text = createFont("chalkboard",100,true);
  textFont(start_text,36);
  fill(0xFF000000);
  textAlign(CENTER);
  text("Stand on Board to Begin!",width/2,height/10);
}

void play() {
  if(new_target) {
    // translate from origin to center: 
    new_target_roll = random(-30,30);
    new_target_pitch = random(30,30);
    //target_roll = -5;
    //target_pitch = -5;
    new_target = false;  
  }
  
  drawtarget();
  drawboard();
  //float alphaA = map(mouseX, 0, width, 0, 255);
  //float alphaB = map(mouseY, 0, height, 0, 255);
  tint(255, 255);
  image(target, 0, 0);
  //tint(board_tint, 128);
   tint(tintr,tintg,tintb,128);
  image(board, 0, 0);
  if((moving_roll || moving_pitch) && nailed_it){
    nailed_it_text = createFont("chalkboard",100,true);
    textFont(nailed_it_text,36);
    
    //translate(width/2,8*height/10);
    textAlign(CENTER);
    fill(0xFF000000);
    text(nailed_it_strings[i],width/2,8*height/10);
    
    if(!printed_text){
      i++;
      if(i>=num_string)
        i = 0;
    }
  printed_text = true;
  }
}

void drawtarget() {
  target.beginDraw();
  target.lights();
  target.clear();
  target.noStroke();
  target.translate(width/2, height/2);
  moving_roll = true;
  moving_pitch = true;
  
  if(abs(new_target_roll - target_roll) < 0.5) {
    print("stopped rolling \n");
    moving_roll = false;
  }
  else if(new_target_roll > target_roll){
    print("here");
    target_roll += 0.5;
  }
  else if(new_target_roll < target_roll){
    print("there");
    target_roll -= 0.5;
  }
  
   if (abs(new_target_pitch - target_pitch) < 0.5) {
    print("Stopped pitching \n");
    moving_pitch = false;
  }
  else if(new_target_pitch > target_pitch){
    target_pitch += 0.5;
  }
  else if(new_target_pitch < target_pitch){
    target_pitch -= 0.5;
  }
  
  target.rotateX(radians(target_roll + 90));
  target.rotateY(radians(target_pitch));
  target.rotateZ(radians(90));
  //target.box(80);
  //target.fill(#E014A3);
  //target.setFill(color(255,0,0));
  //target.ellipse(0, 0, width/2, width/2);
  target.shape(SurfBoard);
  target.endDraw();
}

void drawboard() {
  board.beginDraw();
  board.lights();
  board.clear();
  board.noStroke();
  board.translate(width/2, height/2);
  board.rotateX(radians(roll + 90));
  board.rotateY(radians(pitch) );
  board.rotateZ(radians(90));
  if((abs(pitch - target_pitch) < eps) && abs(roll - target_roll) < eps && !moving_pitch && !moving_roll){ 
    if(!holding){
       hold_time = millis();
       print("start hold");
    }
     holding = true;
     tintr = 0;
     tintb = 145;
     tintg = 255;
  }
  else {
    holding = false;
    tintr = 255;
    tintb = 255;
    tintg = 255;
  }
  //board.ellipse(0, 0, width/2, width/2); 
  board.shape(SurfBoard);
  board.endDraw();
  time = millis();
  if((time - hold_time >= hold) && holding){
    print("holding done");
    new_target = true;
    nailed_it = true;
    printed_text = false;
  }
}

/*
void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
  myString = trim(myString);
  
  //print(myString,"\n");
  
  if(!myString.contains(",")){
    state = int(myString);
    //print("State:",state,"\n");
  }
    
  //String state[] = split(\n
  // split the string at the commas 
  String items[] = split(myString, ','); 
  if (items.length > 1) {
      pitch = float(items[0]);
      roll = float(items[1]); 
      //print("Pitch: ",pitch,"\tRoll: ",roll,"\n");
    }
  } 
}*/