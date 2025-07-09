/**
 * The logic for the Simon Application. Handles all level phases and emissions to the view.
 *
 * Authors: John Gibb & Hudson Dalby
 * Version: 3/13/2025
 *
 **/

#ifndef MODEL_H
#define MODEL_H

#include <QObject>
#include <QTimer>
#include <QMediaPlayer>
#include <QAudioOutput>
#include <QDir>

/**
 * @brief The Model class that handles all game logic and communication with the view.
 */
class Model : public QObject
{
    Q_OBJECT

private:

    // Variables

    /**
     * @brief progress The progress the player has made in each level. Used to update the progress bar.
     */
    int progress;

    /**
     * @brief level The level the player is currently playing.
     */
    int level;

    /**
     * @brief delayTime The delay time for each button flash during the pre-level phase.
     */
    int delayTime;

    /**
     * @brief pattern The pattern the game will display and expect from the player.
     */
    int* pattern;

    /**
     * @brief soundEnabled A boolean indicating if the sound is enabled or disabled.
     */
    bool soundEnabled;

    /**
     * @brief dimRed A constant for the red button's stylesheet that turns the button into a faded red color.
     */
    const QString dimRed = "background-color: rgba(255, 0, 0, 35); border: 5px black;";

    /**
     * @brief dimBlue A constant for the blue button's stylesheet that turns the button into a faded blue color.
     */
    const QString dimBlue = "background-color: rgba(0, 0, 255, 35); border: 5px black;";

    /**
     * @brief normalRed A constant for the red button's stylesheet that turns the button into a deep red color.
     */
    const QString normalRed = "background-color: rgba(255, 0, 0, 255); border: 5px black;";

    /**
     * @brief normalBlue A constant for the blue button's stylesheet that turns the button into a deep blue color.
     */
    const QString normalBlue = "background-color: rgba(0, 0, 255, 255); border: 5px black;";

    /**
     * @brief buttonSound The sound played after each correct button press.
     */
    QMediaPlayer* buttonSound;

    /**
     * @brief levelPassSound The sound played after each level is completed.
     */
    QMediaPlayer* levelPassSound;

    /**
     * @brief failSound The sound played after an incorrect guess by the player.
     */
    QMediaPlayer* failSound;

    /**
     * @brief soundOut The output for the buttonSound.
     */
    QAudioOutput* buttonSoundOut;

    /**
     * @brief levelPassSoundOut The output for the levelPassSound.
     */
    QAudioOutput* levelPassSoundOut;

    /**
     * @brief failSoundOut The output for the failSound.
     */
    QAudioOutput* failSoundOut;

    // Helper methods

    /**
     * @brief calculateDelayTime A helper method for calculating the delay time for each button flash and enabling the game buttons.
     * @param phase The "phase" during the pre-level showing of the pattern.
     * @return The delay time for this function call.
     */
    int calculateDelayTime(int phase);

    /**
     * @brief flashButtons A helper method for flashing the red and blue buttons. The pattern indicates which button will be flashed,
     * then this method changes the button to a normal color, then back to the dim color after the delay time has passed.
     * @param phase The "phase" during the pre-level showing of the pattern.
     */
    void flashButtons(int phase);

    /**
     * @brief endGame Slot to end the game in the case of an incorrect guess by the player. Resets the game progress, enables
     * the Start button, and disables the game buttons until the game is started again.
     */
    void endGame();

    /**
     * @brief newLevel Slot that handles the pre-level phase for each new level. Sets progress to zero, decreases the delay time between
     * each button flash, adds to the existing pattern, flashes the buttons according to the pattern, then sets up the game to take in
     * user input.
     */
    void newLevel();

public:

    /**
     * @brief Model Constructor for a model object. Initializes default values for all instance variables and sets up the audio elements.
     * @param parent The parent QObject.
     */
    explicit Model(QObject *parent = nullptr);

    /**
     * The destructor for a Model object. Deletes the dynamic pattern array and the sound elements.
     */
    ~Model();

public slots:

    /**
     * @brief redButtonClicked Event handler for when the red button is clicked. Checks the validity against the pattern
     * array, plays the appropriate sound, and updates the labels, progress and level accordingly.
     */
    void redButtonClicked();

    /**
     * @brief blueButtonClicked Event handler for when the blue button is clicked. Checks the validity against the pattern
     * array, plays the appropriate sound, and updates the labels, progress, and level accordingly.
     */
    void blueButtonClicked();

    /**
     * @brief startGame Slot to start the first level and disable the start button.
     */
    void startGame();

    /**
     * @brief toggleSound Slot that handles the toggle sound button. Turns the game sound on or off depending on the current state of
     * the soundEnabled variable.
     */
    void toggleSound();

signals:

    /**
     * @brief changeLabel Changes the displayed text above the game buttons. Indicates the level state and shows helpful information
     * to the player.
     * @param newLabel The new text to be displayed.
     */
    void changeLabel(const QString newLabel);

    /**
     * @brief setProgressBar Sets the progress bar to the updated value.
     * @param num The updated value for the progress bar.
     */
    void setProgressBar(int num);

    /**
     * @brief toggleStart Used to toggle the start button on or off.
     * @param onOrOff Boolean indicating the new state of the start button.
     */
    void toggleStart(bool onOrOff);

    /**
     * @brief toggleGameButtons Used to toggle the red and blue buttons on or off.
     * @param onOrOff Boolean indicating the new state of the red and blue buttons.
     */
    void toggleGameButtons(bool onOrOff);

    /**
     * @brief changeRedColor Changes the stylesheet of the red button.
     * @param rGBAVAlue The new contents of the red button's stylesheet.
     */
    void changeRedColor(QString rGBAVAlue);

    /**
     * @brief changeBlueColor Changes the stylesheet of the blue button.
     * @param rGBAVAlue The new contents of the blue button's stylesheet.
     */
    void changeBlueColor(QString rGBAVAlue);

    /**
     * @brief soundButtonPressed Changes the displayed state of the sound button.
     * @param enabled Boolean indicating if the sound is on or off.
     */
    void soundButtonPressed(const QString soundOnOrOff);

};

#endif // MODEL_H
