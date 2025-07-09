/**
 * The view of the Simon Application
 *
 * Authors: John Gibb & Hudson Dalby
 * Version: 3/13/2025
 *
 **/

#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "model.h"

QT_BEGIN_NAMESPACE
namespace Ui {

/**
 * Forward declaration of the MainWindow class.
 */
class MainWindow;
}
QT_END_NAMESPACE

/**
 * @brief The MainWindow class that connects elements between the view and model of this application.
 */
class MainWindow : public QMainWindow
{
    Q_OBJECT

public:

    /**
     * @brief MainWindow The constructor for the main window of the Simon game. Sets up all connections between the
     * game's model and this window.
     * @param model The model to connect to this game's view.
     * @param parent The QWidget parent object.
     */
    MainWindow(Model* model, QWidget *parent = nullptr);

    /**
     * The destructor for the main window, deletes the games UI.
     */
    ~MainWindow();

private:

    /**
     * @brief ui The UI for this Simon game
     */
    Ui::MainWindow *ui;

};
#endif // MAINWINDOW_H
