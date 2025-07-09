#include "mainwindow.h"
#include <QApplication>

/**
 * The entry point of the Simon Application
 *
 * Team members: John Gibb & Hudson Dalby
 * John's Github username: jgibby16
 * Hudson's Github username: hud13
 * Github URL: https://github.com/UofU-CS3505/cs3505-assignment6-H-Dalby
 *
 * The creative element we decided to add was game sounds for correct player input, level completion,
 * and incorrect guesses or a failed level. We also added the ability to toggle these game sounds on
 * and off through a button press.
 *
 * Version: 3/13/2025
 **/

/*
 * Main method that initializes the application, model, and main window for the game, then displays and executes the game.
*/
int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    Model m;
    MainWindow w(&m);
    w.show();
    return a.exec();
}
