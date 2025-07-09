/**
 * The Model of the Simon Application
 *
 * Authors: John Gibb & Hudson Dalby
 * Version: 3/13/2025
 **/

#include "model.h"

/*
 * Constructor for a model object. Initializes default values for the level progress, level, delay time for the speed
 * of each pre-level phase, button pattern, and the sounds that are played while playing the Simon game.
*/
Model::Model(QObject *parent)
    : QObject{parent},
    progress(0),
    level(0),
    delayTime(1200),
    soundEnabled(true)
{
    pattern = new int[2]();

    // Initialization for the audio elements
    buttonSoundOut = new QAudioOutput(this);
    levelPassSoundOut = new QAudioOutput(this);
    failSoundOut = new QAudioOutput(this);

    buttonSound = new QMediaPlayer(this);
    levelPassSound = new QMediaPlayer(this);
    failSound = new QMediaPlayer(this);

    buttonSound->setAudioOutput(buttonSoundOut);
    levelPassSound->setAudioOutput(levelPassSoundOut);
    failSound->setAudioOutput(failSoundOut);

    buttonSound->setSource(QUrl("qrc:/sounds/buttonClick.mp3"));
    levelPassSound->setSource(QUrl("qrc:/sounds/levelPass.mp3"));
    failSound->setSource(QUrl("qrc:/sounds/fail.mp3"));
}

/*
 * Destructor for all heap allocated resources.
*/
Model::~Model(){
    delete[] pattern;
    delete buttonSoundOut;
    delete buttonSound;
    delete levelPassSoundOut;
    delete levelPassSound;
    delete failSoundOut;
    delete failSound;
}

/*
 * Event handler for the red button being clicked
 * Checks color against the stored pattern and triggers the appropriate event.
*/
void Model::redButtonClicked(){
    if (pattern[progress] == 0){
        progress++;
        if (soundEnabled && progress != level){
            buttonSound->play();
        }
        emit setProgressBar(100 * progress/level);
        if (progress == level) {
            emit toggleGameButtons(false);
            if (soundEnabled){
                levelPassSound->play();
            }
            emit changeLabel("Level " + QString::number(level) + " complete!");
            QTimer::singleShot(1000, this, &Model::newLevel);
        }
    }
    else {
        emit changeLabel("Incorrect! Game over.");
        endGame();
    }
}

/*
 * Event handler for the blue button being clicked.
 * Checks color against the stored pattern and triggers appropriate event.
*/
void Model::blueButtonClicked(){
    if (pattern[progress] == 1){
        progress++;
        if (soundEnabled && progress != level){
            buttonSound->play();
        }
        emit setProgressBar(100 * progress/level);
        if (progress == level){
            emit toggleGameButtons(false);
            if (soundEnabled){
                levelPassSound->play();
            }
            emit changeLabel("Level " + QString::number(level) + " complete!");
            QTimer::singleShot(1000, this, &Model::newLevel);
         }
    }
    else {
            emit changeLabel("Incorrect! Game over.");
            endGame();
        }
    }

/*
 * Slot to start the game and disable to start button.
*/
void Model::startGame(){
    emit toggleStart(false);
    newLevel();
}

/*
 * Method that assists with ending the game, resetting progress and disabling game buttons until the game is restarted.
 */
void Model::endGame(){
    emit toggleStart(true);
    if (soundEnabled){
        failSound->play();
    }
    progress =  0;
    level = 0;
    delayTime = 1200;
    emit changeLabel("Game Over. Try again.");
    emit toggleGameButtons(false);
}

/*
 * Method that handles the necessary setup for a new level.
*/
void Model::newLevel(){
    emit setProgressBar(0);
    progress = 0;
    level = level + 1;
    emit changeLabel((QString)"Starting level " + QString::number(level) + ". Memorize the pattern!");
    if (delayTime >= 200){
        delayTime -= 100;
    }
    // Dim button color
    emit changeRedColor(dimRed);
    emit changeBlueColor(dimBlue);

    // Add onto existing pattern for each new level
    int val = rand() % 2;
    pattern[level - 1] = val;

    // Loop through the pattern and flash buttons
    for(int phase = 0; phase < level; phase++){
        flashButtons(phase);
    }

    // Enable user interaction after pre-level phase and restore button color
    QTimer::singleShot(calculateDelayTime(level), this, [this](){
        emit toggleGameButtons(true);});
    QTimer::singleShot(calculateDelayTime(level), this, [this](){
        emit changeRedColor(normalRed);});
    QTimer::singleShot(calculateDelayTime(level), this, [this](){
        emit changeBlueColor(normalBlue);});
    QTimer::singleShot(calculateDelayTime(level), this, [this](){
        emit changeLabel("Recall the pattern!");});
}

/*
 * Private helper method for flashing the red and blue buttons based on the generated pattern.
*/
void Model::flashButtons(int phase){
    if(pattern[phase] == 0){
        QTimer::singleShot(calculateDelayTime(phase), this, [this](){
            emit changeRedColor(normalRed);
        });
        QTimer::singleShot(calculateDelayTime(phase) + delayTime, this, [this](){
            emit changeRedColor(dimRed);
        });
    }
    else{
        QTimer::singleShot(calculateDelayTime(phase), this, [this](){
            emit changeBlueColor(normalBlue);
        });
        QTimer::singleShot(calculateDelayTime(phase) + delayTime, this, [this](){
            emit changeBlueColor(dimBlue);
        });
    }
}

/*
 * Private helper method for calculating the delay time for each timer.
*/
int Model::calculateDelayTime(int phase){
    return (phase * delayTime * 2) + delayTime;
}

/*
 * Slot to handle the toggle sound button press.
 */
void Model::toggleSound(){
    soundEnabled = !soundEnabled;
    emit soundButtonPressed(soundEnabled ? "Sound: ON" : "Sound: OFF");

    // Stops the sounds if they are currently playing and sound is disabled.
    if (!soundEnabled){
        buttonSound->stop();
        levelPassSound->stop();
        failSound->stop();
    }
}
