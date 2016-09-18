import processing.sound.*;

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
int hold = 1000;
int hold_time, time;
int time_to_fail=20;
int time_to_fail_decrease=0;
//sound declaration
SoundFile ding, surf_music;
SoundFile wack, so_pitted, bro, whoo, so_sick;
int sound_length = 4;
int sound = 0;

//score
int score=0;

//average
int angle_count = 0;
int samples = 10;
float[] pitch_array = new float[10];
float[] roll_array = new float[10];
float total_roll = 0;
float total_pitch = 0;

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
  SurfBoard.scale(1.8);
  SurfBoard.setFill(color(255,255,255));
  
  vector_target = new PVector(0,0);//pitch,roll
  vector_player = new PVector(0,0);//pitch,roll
  vector_delta = new PVector(0,0);
  vector_target_draw =  new PVector(0,0);
  
  //sound stuff
  ding = new SoundFile(this,"ding.mp3");
  surf_music = new SoundFile(this,"surf_music.mp3");
  wack = new SoundFile(this,"wack.mp3");
  so_pitted = new SoundFile(this,"so_pitted.mp3");
  bro = new SoundFile(this,"bro.mp3");
  whoo = new SoundFile(this,"whoo.mp3");
  so_sick = new SoundFile(this,"so_sick.mp3");
  surf_music.loop();  
} 

void draw() {
  //////////////////////////average
  if(angle_count > samples-1)
    angle_count = 0;
  pitch_array[angle_count] = pitch;
  roll_array[angle_count] = roll;
  
  angle_count++;
  
  for(int average_count = 0; average_count < samples; average_count++) {
    total_pitch += pitch_array[average_count];
    total_roll += roll_array[average_count];
  }

  vector_player.set(total_pitch/samples,total_roll/samples);
  total_pitch = 0;
  total_roll = 0;
  ///////////////////////end of averaging
  
  //background(wave);
  //state machine for the game
  switch(state) {
  case 0: //Waiting for player to stand over board
    animation0.display(0, 0);
    if (feetsensor==1) {
      play();
      state=1;
      time_level = time_to_fail+(int(millis()/1000));//TODO, 20 as variable
      time_to_fail_decrease=0;
      score = 0;
    } else {
      start_screen();
    }
    break;
  case 1: //player playing
    animation1.display(0, 0);
    if (feetsensor==0) {
      state=3;       
    } else {
      play();
    }
    //if time hits 0, you lose
    if (time_game<=0){
      state=3;
    }

    if (score==4){ //goes to winning score
      state=5;
    }
    break;
  case 3: //wipeout
      //change color screen when target is far
    tint(255,255,255,255);
    animation2.display(0, 0);
    textFont(font_game, 36);
    text("Score: ", width*1/10, height*1/10);  
    text(score, width*1/10, height*1.7/10);     
    textFont(font_game, 50);
    text("Wipe-out!!", width*5/10, height*8/10);     
    if (feetsensor==1) {
      play();
      state=4;
    }     
    break;
  case 4: //transition state before game goes back to initial page, player must step up and down once, (this can be implemented with a timer)
    animation2.display(0, 0);
    textFont(font_game, 36);
    text("Score: ", width*1/10, height*1/10);  
    text(score, width*1/10, height*1.7/10);     
    textFont(font_game, 50);
    text("Wipe-out!!", width*5/10, height*8/10); 
    if (feetsensor==0) {
      state=0;
    }     
    break;
  case 5: 
    animation3.display(0, 0);
    textFont(font_game, 36);
    text("Score: ", width*1/10, height*1/10);  
    text(score, width*1/10, height*1.7/10); 
    textFont(font_game, 50);
    text("Perfect Wave!!", width*5/10, height*8/10);     
    
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
  
  if (time_game<=5){
    fill(0xFF000000);
  } else {
    fill(0xFF000000);
  }
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

void drawscore(){
  textFont(font_game, 36);
  text("Score: ", width*1/10, height*1/10);  
  text(score, width*1/10, height*1.7/10);  
}
//////////////////////////////////////////////////////////////////////PLAY
void play() {
  float distance;
  
  if (new_target) {
    vector_target.set(random(-10, 10),random(-6, 6));//pitch,roll
    vector_delta = PVector.sub(vector_target, vector_target_draw);
    vector_delta = vector_delta.div(frame_targe_transition);
    count_target_update = 0;
    // translate from origin to center: 
    new_target = false;
  }

 //add small updates to target
 if (count_target_update<frame_targe_transition) {
   vector_target_draw.add(vector_delta);
   count_target_update++;
   if (nailed_it) {
    textAlign(CENTER);
    fill(0xFFFF0000);
    text(nailed_it_strings[i],width/2,2*height/10);
    fill(0xFF000000);
   }
 }

  //draw target
  drawboardaux(vector_target_draw.x,vector_target_draw.y,255,255,255,255);

  //draw board current position
  drawboardaux(vector_player.x,vector_player.y,tintr, tintg , tintb , 128);

  //check if player is close to target, NEED TO BE FIXED
  distance = PVector.dist(vector_target,vector_player);
  if (distance<5.0){
    //change color screen when target is close
    tintr= 0;
    tintb= 0;
    tintg =255;
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
    //change color screen when target is far
    tintr= 255;
    tintb= 255;
    tintg =255;    
  }  
  time = millis();
  //check if player has hold postion for a "hold" time
  if ((time - hold_time >= hold) && holding) {
    score = score+2;
    i++;
    if(i>=num_string)
      i = 0;     
    //change color screen when target is far
    tintr= 255;
    tintb= 255;
    tintg =255;    
    tint(255,255,255,255);
    print("holding done");
    new_target = true;
    nailed_it = true;
    holding=false;
    time_to_fail_decrease++;
    time_level = time_to_fail+(int(millis()/1000))-time_to_fail_decrease;//TODO, 20 as variable
    //sound stuff
    ding.play();
    if(sound == 0)
      wack.play();
      else if(sound == 1)
        so_pitted.play();
              else if(sound == 2)
                bro.play();
                else if(sound == 3)
                  whoo.play();
                  else if(sound == 4)
                    so_sick.play();
    sound++;
    if(sound > sound_length)
      sound = 0;    
  } 
 
  //draw timer in screen
  drawtimer();
  drawscore();
}

void drawboardaux(float pitch_draw,float roll_draw, float vR, float vG,  float vB,  int alpha ) {
  //this funtion draw a surfboard for given pitch and roll angles, also change color and transparency 
  boardaux.beginDraw();
  boardaux.lights();
  boardaux.clear();
  boardaux.noStroke();
  boardaux.translate(width/4, height*3/4);

  boardaux.rotateX(radians(roll_draw + 90));
  boardaux.rotateY(radians(pitch_draw) );
  boardaux.rotateZ(radians(180));

  boardaux.shape(SurfBoard);
  boardaux.endDraw();
  tint(vR,vG,vB,alpha);
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