/**
 * StartMenu is the initial screen upon launching the project. Allows the user to make a new project.
 *
 * @author Hudson Dalby
 * @date March 31, 2025
 * @reviewer Pierce Jones & Matthew Shaw
 */
#ifndef STARTMENU_H
#define STARTMENU_H

#include <QWidget>
#include "ui_startmenu.h"
#include "spriteeditor.h"
#include "spritemodel.h"
#include <QFileDialog>

QT_BEGIN_NAMESPACE
namespace Ui {
class startMenu;
}
QT_END_NAMESPACE

/**
 * @class StartMenu
 * @brief Represents the start menu UI for creating and loading projects.
 *
 * @author Hudson Dalby
 * @date March 31, 2025
 * @reviewer Pierce Jones
 */
class StartMenu : public QWidget
{
    Q_OBJECT

public:
    /**
     * @brief Constructs a StartMenu object.
     * @param parent Pointer to the parent widget.
     */
    explicit StartMenu(QWidget *parent = nullptr);

    /**
     * @brief Destroys the StartMenu object.
     */
    ~StartMenu();

private slots:
    /**
     * @brief Handles making a new project when clicked.
     */
    void on_projectNew_clicked();

    /**
     * @brief Handles loading a project when clicked.
     */
    void on_projectLoad_clicked();

    /**
     * @brief Updates the project size label.
     * @param size The new size value.
     */
    void updateProjectSizeLabel(int size);

    /**
     * @brief Updates the project size tooltip.
     * @param size The new size value.
     */
    void updateProjectSizeTip(int size);


private:
    Ui::startMenu *ui;
};

#endif //STARTMENU_H
