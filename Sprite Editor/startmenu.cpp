/**
 * The implementation of startmenu.h, represents a functional start menu for the sprite editor.
 *
 * @author Hudson Dalby
 * @date March 31, 2025
 * @reviewer Pierce Jones & Matthew Shaw
 */

#include "startmenu.h"

StartMenu::StartMenu(QWidget *parent)
    : QWidget(parent)
    , ui(new Ui::startMenu)
{
    ui->setupUi(this);

    connect(ui->projectSizeSelector, &QSlider::valueChanged, this, &StartMenu::updateProjectSizeLabel);
    connect(ui->projectSizeSelector, &QSlider::valueChanged, this, &StartMenu::updateProjectSizeTip);
}

StartMenu::~StartMenu()
{
    delete ui;
}

void StartMenu::on_projectNew_clicked()
{
    int userSize = ui->projectSizeSelector->value();

    SpriteModel *model = new SpriteModel(userSize, this);

    SpriteEditor *editor = new SpriteEditor(model);
    editor->show();

    close();
}

void StartMenu::on_projectLoad_clicked()
{
    QString fileName = QFileDialog::getOpenFileName(this,
                                                    tr("Load Project"),
                                                    QString(),
                                                    tr("Sprite Projects (*.ssp)"));

    if (!fileName.isEmpty())
    {
        SpriteModel *model = new SpriteModel(2, this);
        model->loadProject(fileName);

        SpriteEditor *editor = new SpriteEditor(model);
        editor->show();

        close();
    }
}

void StartMenu::updateProjectSizeLabel(int size)
{
    ui->projectSizeLabel->setText("Project Size (" + QString::number(size) + ")");
}

void StartMenu::updateProjectSizeTip(int size)
{
    ui->projectSizeLabel2->setText("(Created sprite project will be " + QString::number(size) + "x" +
                                   QString::number(size) + "pixels)");
}
