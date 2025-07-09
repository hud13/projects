/**
 * Auto generated main class that makes the program work
 *
 * @author Auto generated, modified slightly to open start menu.
 * @date March 31, 2025
 * @reviewer Pierce Jones
 */

#include "startmenu.h"

#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);

    StartMenu s;
    s.show();
    return a.exec();
}
