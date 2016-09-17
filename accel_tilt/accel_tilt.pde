//Graphic variables
PGraphics boardaux;
Animation animation0,animation1, animation2,animation3;
PShape SurfBoard;
//serial port setting variables
import processing.serial.*;                  // import the serial lin
Serial myPort;                               // the serial port
//variables from serial port (coming from RedBear Duo)
float pitch, roll; 
int feetsensor;    
//board position variables
int count_target_update, frame_targe_transition = 10;
PVector vector_target, vector_player, vector_delta, vector_target_draw;
float target_pitch, target_roll;
float new_target_pitch, new_target_roll;
boolean new_target = true;
boolean holding = false;
boolean nailed_it = false;
//state variables
int state = 0;
boolean moving_roll = true, moving_pitch = true;
String[] nailed_it_strings = {"Nailed It!", "Radical!", "Get Pitted!", "Gnarly!", "Killing It!"};
int num_string = 5;
int i = 0;
boolean printed_text = false;
float eps = 20;
float tintr = 255, tintb = 255, tintg = 255;
//font variables
PFont font_game;
//time control variables
int time_game;
int time_level=20;
int hold = 2000;
int hold_time, time;


void setup() {
  size(736, 552, P3D); 
  frameRate(15);
  font_game = createFont("zorque", 100, true);
  animation0 = new Animation("catstep/tmp-", 38);
  animation1 = new Animation("tuberiding/tmp-", 183);
  animation2 = new Animation("wipeout/tmp-", 31);
  animation3 = new Animation("surfrelax/tmp-", 30);

  boardaux = createGraphics(width, height, P3D);

  myPort = new Serial(this, Serial.list()[0], 9600);
  // only generate a serial event when you get a newline: 
  myPort.bufferUntil('\n');
  SurfBoard = loadShape("SurfBoard.obj");
  
  vector_target = new PVector(0,0);//pitch,roll
  vector_player = new PVector(0,0);//pitch,roll
  vector_delta = new PVector(0,0);
  vector_target_draw =  new PVector(0,0);
} 

void draw() {
  vector_player.set(pitch,roll);
  //background(wave);
  //state machine for the game
  switch(state) {
  case 0: //Waiting for player to stand over board
    animation0.display(0, 0);
    if (feetsensor==1) {
      play();
      state=1;
    } else {
      start_screen();
    }
    break;
  case 1: //player playing
    animation1.display(0, 0);
    if (feetsensor==0) {
      start_screen();
      state=3;
    } else {
      play();
    }
    break;
  case 3: //wipeout
    animation2.display(0, 0);
    if (feetsensor==1) {
      play();
      state=4;
    }     
    break;
  case 4: //transition state before game goes back to initial page, player must step up and down once, (this can be implemented with a timer)
    animation2.display(0, 0);
    if (feetsensor==0) {
      state=0;
    }     
    break;
  default:
    println("default");   // Does not execute
    break;
  }
}

void start_screen() {
  textFont(font_game, 36);
  fill(0xFF000000);
  textAlign(CENTER);
  text("Stand on Board to Begin!", width/2, height/6);
  target_roll = 0;
  target_pitch = 0;
}

void drawtimer(){
  textFont(font_game, 36);
  String time;
  time_game = time_level-(int(millis()/1000));
  time = nf(time_game , 2); 
  text("Timer: ", width*9/10, height*1/10);  
  text(time, width*9/10, height*1.7/10);  
}
//////////////////////////////////////////////////////////////////////PLAY
void play() {
  float distance;
  
  if (new_target) {
    vector_target = PVector.mult(PVector.random2D(),15);
    vector_delta = PVector.sub(vector_target, vector_target_draw);
    vector_delta = vector_delta.div(frame_targe_transition);
    count_target_update = 0;
    // translate from origin to center: 
    new_target_roll = random(-30, 30);
    new_target_pitch = random(-30, 30);
    new_target = false;
  }

 //add small updates to target
 if (count_target_update<frame_targe_transition) {
   vector_target_draw.add(vector_delta);
   count_target_update++;
   if (nailed_it) {
     text("MATCHED", width/2, 8*height/10); //just to check
   }
 }

  //draw target
  drawboardaux(vector_target_draw.x,vector_target_draw.y,255,255,255,255);

  //draw board current position
  drawboardaux(vector_player.x,vector_player.y,255,255,255, 128);

  //check if player is close to target, NEED TO BE FIXED
  distance = PVector.dist(vector_target,vector_player);
  if (distance<3.0){
    
    if (!holding) {
      holding=true;
      hold_time = millis();
      print("start hold");
    }    
  } else {
    holding = false;
    if (count_target_update==frame_targe_transition) {
      nailed_it = false;
    }
  }  
  time = millis();
  if ((time - hold_time >= hold) && holding) {
    print("holding done");
    new_target = true;
    nailed_it = true;
    holding=false;
    time_level = time_level+(int(millis()/1000));
  } 
 
  //draw timer in screen
  drawtimer();
}

void drawboardaux(float pitch_draw,float roll_draw, float v1, float v2,  float v3,  int alpha ) {
  //this funtion draw a surfboard for given pitch and roll angles, also change color and transparency 
  boardaux.beginDraw();
  boardaux.lights();
  boardaux.clear();
  boardaux.noStroke();
  boardaux.translate(width/2, height/2);

  boardaux.rotateX(radians(roll_draw + 90));
  boardaux.rotateY(radians(pitch_draw) );
  boardaux.rotateZ(radians(90));

  boardaux.shape(SurfBoard);
  boardaux.endDraw();
  tint(v1,v2,v3,alpha);
  image(boardaux, 0, 0);

}

void serialEvent(Serial myPort) {
  // read the serial buffer:
  String myString = myPort.readStringUntil('\n');
  // if you got any bytes other than the linefeed: 
  if (myString != null) {
    myString = trim(myString);
    String items[] = split(myString, ','); 
    pitch = float(items[0]);
    roll = float(items[1]);
    feetsensor = int(items[2]);
  }
}