/**
 * The view of the Simon Game
 *
 * Authors: John Gibb & Hudson Dalby
 * Version: 3/13/2025
 *
 **/

#include "mainwindow.h"
#include "ui_mainwindow.h"

/*
 * Constructor for the Main Window Widget on startup.
*/
MainWindow::MainWindow(Model* model, QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    // Sets the buttons to be disabled by default
    ui->Blue->setDisabled(true);
    ui->Red->setDisabled(true);

    connect(ui->Blue,
            &QPushButton::clicked,
            model,
            &Model::blueButtonClicked);

    connect(ui->Red,
            &QPushButton::clicked,
            model,
            &Model::redButtonClicked);

    connect(ui->startButton,
            &QPushButton::clicked,
            model,
            &Model::startGame);

    connect(ui->soundButton,
            &QPushButton::clicked,
            model,
            &Model::toggleSound);

    // From model to view

    connect(model,
            &Model::setProgressBar,
            ui->progressBar,
            &QProgressBar::setValue);

    connect(model,
            &Model::changeLabel,
            ui->label,
            &QLabel::setText);

    connect(model,
            &Model::toggleStart,
            ui->startButton,
            &QPushButton::setEnabled);

    connect(model,
            &Model::toggleGameButtons,
            ui->Red,
            &QPushButton::setEnabled);

    connect(model,
            &Model::toggleGameButtons,
            ui->Blue,
            &QPushButton::setEnabled);

    connect(model,
            &Model::changeRedColor,
            ui->Red,
            &QPushButton::setStyleSheet);

    connect(model,
            &Model::changeBlueColor,
            ui->Blue,
            &QPushButton::setStyleSheet);

    connect(model,
            &Model::soundButtonPressed,
            ui->soundButton,
            &QPushButton::setText);

}

/*
 * Destructor for main window, deletes game assets.
*/
MainWindow::~MainWindow()
{
    delete ui;
}
