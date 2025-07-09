/**
 * A Sprite Editor program that allows users to paint pixels on a surface and create
 * animated sprites from multiple frames.
 *
 * @author Golightly Chamberlain, Pierce Jones, Hudson Dalby, Matthew Shaw, Cheuk Yin Lau, and John Gibb
 * @date March 31, 2025
 * @reviewer John Gibb and Hudson Dalby and Pierce Jones
 */

#include "spriteeditor.h"

SpriteEditor::SpriteEditor(SpriteModel *model, QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::SpriteEditor)
    , model(model)
    , pencil(new Pencil())
{
    animationTimer = new QTimer(this);
    animationTimer->start(200);  // milliseconds
    QShortcut *copyShortcut = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_C), this);
    QShortcut *pasteShortcut = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_V), this);
    QShortcut *undoShortcut = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_Z), this);
    QShortcut *redoShortcut = new QShortcut(QKeySequence(Qt::CTRL | Qt::Key_Y), this);
    ui->setupUi(this);

    // View -> Model
    connect(ui->returnToMenu,
            &QPushButton::clicked,
            this,
            &SpriteEditor::backToMainMenu);
    connect(ui->addFrame,
            &QPushButton::clicked,
            this,
            &SpriteEditor::handleAdd);
    connect(ui->previewStopStart,
            &QPushButton::clicked,
            this,
            &SpriteEditor::toggleAnimation);

    // View -> View
    connect(ui->saveButton,
            &QToolButton::clicked,
            this,
            &SpriteEditor::saveProject);
    connect(ui->redValueSelector,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updateColorPreview);
    connect(ui->greenValueSelector,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updateColorPreview);
    connect(ui->blueValueSelector,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updateColorPreview);
    connect(ui->transparencySelector,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updateColorPreview);
    connect(ui->deleteFrame,
            &QPushButton::clicked,
            this,
            &SpriteEditor::deleteFromFrameListWidget);
    connect(ui->frameDisplay,
            &QListWidget::itemClicked,
            this,
            &SpriteEditor::switchFrame);
    connect(ui->frameDisplay,
            &QListWidget::itemClicked,
            ui->deleteFrame,
            &QPushButton::setEnabled);
    connect(ui->penTool,
            &QPushButton::clicked,
            this,
            &SpriteEditor::penSelected);
    connect(ui->eraserTool,
            &QPushButton::clicked,
            this,
            &SpriteEditor::eraserSelected);
    connect(ui->penSize,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::penSizeChanged);
    connect(ui->penSize,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updatePenSizeLabel);
    connect(ui->eraserSize,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::eraserSizeChanged);
    connect(ui->eraserSize,
            &QSlider::valueChanged,
            this,
            &SpriteEditor::updateEraserSizeLabel);
    connect(ui->eyedropperTool,
            &QPushButton::toggled,
            this,
            &SpriteEditor::eyedropperToggled);

    // Model -> View
    connect(model,
            &SpriteModel::updateFrameMenu,
            this,
            &SpriteEditor::updateFrameIcons);
    connect(model,
            &SpriteModel::displaySprite,
            this,
            &SpriteEditor::frameFocus);
    connect(model,
            &SpriteModel::disableDeleteButton,
            ui->deleteFrame,
            &QPushButton::setDisabled);
    connect(model->getFrame(currentFrameIndex),
            &Sprite::disableDropper,
            this,
            &SpriteEditor::dropperFinished);

    // Copy / Paste shortcuts
    connect(copyShortcut,
            &QShortcut::activated,
            this,
            &SpriteEditor::handleCopy);
    connect(pasteShortcut,
            &QShortcut::activated,
            this,
            &SpriteEditor::handlePaste);

    // Undo / Redo buttons and shortcuts
    connect(ui->undoButton,
            &QPushButton::clicked,
            this,
            &SpriteEditor::handleUndo);
    connect(ui->redoButton,
            &QPushButton::clicked,
            this,
            &SpriteEditor::handleRedo);
    connect(undoShortcut,
            &QShortcut::activated,
            this,
            &SpriteEditor::handleUndo);
    connect(redoShortcut,
            &QShortcut::activated,
            this,
            &SpriteEditor::handleRedo);

    // Animation timer
    connect(animationTimer,
            &QTimer::timeout,
            this,
            &SpriteEditor::nextAnimationFrame);

    Sprite *frame = model->getFrame(currentFrameIndex);
    if (frame)
    {
        connect(frame,
                &Sprite::changeColor,
                this,
                &SpriteEditor::updateColorSelector);
    }
    connect(ui->fpsSelector,
            &QDial::valueChanged,
            this,
            &SpriteEditor::updateFPS);

    // default to 1 FPS (1-120 limit)
    ui->fpsSelector->setValue(1);

    // 1 FPS = 1000 ms per frame
    animationTimer->start(1000);

    // set the range for each color selector to 0 - 255
    ui->redValueSelector->setRange(0, 255);
    ui->greenValueSelector->setRange(0, 255);
    ui->blueValueSelector->setRange(0, 255);
    ui->transparencySelector->setRange(0, 255);

    // default values: red and transparency default to 255, others default to 0
    ui->redValueSelector->setValue(255);
    ui->greenValueSelector->setValue(0);
    ui->blueValueSelector->setValue(0);
    ui->transparencySelector->setValue(255);

    // update the preview initially
    updateColorPreview();
    connect(ui->playBackAtActualSize,
            &QPushButton::clicked,
            this,
            [this, model]()
            {
                if (model->getFrameCount() == 0)
                    return;
                QVector<Sprite *> frameVector;
                for (int i = 0; i < model->getFrameCount(); i++)
                {
                    frameVector.append(model->getFrame(i));
                }

                // fps set equal to preview menu.
                PlaybackWindow *playback = new PlaybackWindow(frameVector, ui->fpsSelector->value());
                playback->setAttribute(Qt::WA_DeleteOnClose);
                playback->show();
            }
    );
    currentDrawingSprite = model->getFrame(0);

    // Create first Icon is frame list and setup delete button
    QListWidgetItem *item = new QListWidgetItem();
    item->setSizeHint(QSize(75, 75));
    ui->frameDisplay->addItem(item);
    frameFocus(model->getFrame(0));
    ui->deleteFrame->setDisabled(true);
}

SpriteEditor::~SpriteEditor()
{
    delete ui;
    delete pencil;
    delete animationTimer;
}

void SpriteEditor::backToMainMenu()
{
    StartMenu *menu = new StartMenu();
    menu->show();
    this->close();
}

void SpriteEditor::saveProject()
{
    QString fileName = QFileDialog::getSaveFileName(this,
                                                    tr("Save Project"),
                                                    "",
                                                    tr("Sprite Projects (*.ssp)"));
    if (!fileName.isEmpty())
    {
        model->saveProject(fileName);
    }
}

void SpriteEditor::toggleAnimation()
{
    if (animationTimer->isActive())
        animationTimer->stop();
    else
        animationTimer->start();
}

void SpriteEditor::nextAnimationFrame()
{
    int frameCount = model->getFrameCount();
    if (frameCount == 0)
        return;
    if (currentAnimationFrameIndex > frameCount - 1)
        currentAnimationFrameIndex = 0;
    Sprite *sprite = model->getFrame(currentAnimationFrameIndex);
    QWidget *box = ui->animationPreviewBox;
    showImg(sprite->getImage(), box);
    currentAnimationFrameIndex++;
    currentAnimationFrameIndex %= frameCount;
}

void SpriteEditor::updateFPS(int fps)
{
    // if fps is less than or equal to 0, default to 1 to avoid division by zero.
    if (fps <= 0)
    {
        fps = 1;
        ui->fpsSelector->setValue(1);
    }
    int interval = 1000 / fps; // calculate interval in milliseconds.
    animationTimer->setInterval(interval);
    ui->fpsLabel->setText(QString("FPS: %1").arg(fps));
}

void SpriteEditor::showImg(QImage img, QWidget *box)
{
    std::string tag = "animationCycle";
    for (QLabel *label : box->findChildren<QLabel *>(tag))
    {
        delete label;
    }
    QLabel *label = new QLabel(box);
    label->setObjectName(tag);
    label->setGeometry(box->rect());
    label->setPixmap(
        QPixmap::fromImage(img).scaled(label->size(),
        Qt::KeepAspectRatio,
        Qt::FastTransformation));
    label->show();
}

void SpriteEditor::updateColorSelector(const QColor &color)
{
    ui->redValueSelector->setValue(color.red());
    ui->greenValueSelector->setValue(color.green());
    ui->blueValueSelector->setValue(color.blue());
    ui->transparencySelector->setValue(color.alpha());
    updateColorPreview();
}
void SpriteEditor::updateColorPreview()
{
    // get color component values from the spinboxes
    int r = ui->redValueSelector->value();
    int g = ui->greenValueSelector->value();
    int b = ui->blueValueSelector->value();
    int a = ui->transparencySelector->value();

    // create the color with the provided components
    QColor color(r, g, b, a);
    pencil->setColor(color);

    // update the color preview box by setting its background color
    QPalette pal = ui->colorPreviewBox->palette();
    pal.setColor(QPalette::Window, color);
    ui->colorPreviewBox->setAutoFillBackground(true);
    ui->colorPreviewBox->setPalette(pal);
    ui->colorPreviewBox->setFrameShape(QFrame::Box);
    ui->colorPreviewBox->setLineWidth(1);

    // force a repaint if needed
    ui->colorPreviewBox->update();
}

void SpriteEditor::deleteFromFrameListWidget()
{
    if (ui->frameDisplay->count() != 0 && ui->frameDisplay->currentItem())
    {
        int index = ui->frameDisplay->row(ui->frameDisplay->currentItem());
        delete ui->frameDisplay->takeItem(index);
        model->deleteFrame(index);
    }
    else
    {
        model->deleteFrame(model->getFrameCount() - 1);
    }
    if (currentFrameIndex > 0)
        currentFrameIndex--;
}
void SpriteEditor::updateFrameIcons()
{
    while (ui->frameDisplay->count() < model->getFrameCount())
    {
        QListWidgetItem *item = new QListWidgetItem();
        item->setSizeHint(QSize(75, 75));
        ui->frameDisplay->addItem(item);
    }

    // Update all icons to reflect frames
    for (int i = 0; i < model->getFrameCount(); i++)
    {
        QImage img = model->getFrame(i)->getImage();
        if (!img.isNull())
        {
            QImage updatedIcon = img.scaled(75, 75, Qt::KeepAspectRatio, Qt::FastTransformation);
            ui->frameDisplay->item(i)->setIcon(QIcon(QPixmap::fromImage(updatedIcon)));
        }
        else
        {
            qWarning() << "Null image detected in frame" << i;
        }
    }
}

void SpriteEditor::switchFrame(QListWidgetItem *frame)
{
    updateFrameIcons();
    currentFrameIndex = ui->frameDisplay->row(frame);

    //Display currentSprite in the paint frame
    frameFocus(model->getFrame(currentFrameIndex));
}

void SpriteEditor::frameFocus(Sprite *sprite)
{
    if (!sprite)
        return;
    connect(sprite,
            &Sprite::spriteUpdated,
            this,
            [this]() { updateFrameIcons(); });
    connect(sprite,
            &Sprite::disableDropper,
            this,
            [this](){ ui->eyedropperTool->setChecked(false);});
    for (int i = 0; i < model->getFrameCount(); i++)
    {
        model->getFrame(i)->setMouseTracking(false);
        model->getFrame(i)->hide();
        model->getFrame(i)->setQFrame(nullptr);
    }
    sprite->setQFrame(ui->drawingFrame);
    sprite->setPencil(pencil);
    sprite->setGeometry(ui->drawingFrame->rect());
    sprite->setMouseTracking(true);
    sprite->show();
    currentDrawingSprite = sprite;
}

void SpriteEditor::penSizeChanged()
{
    int size = ui->penSize->value();
    pencil->setPencilSize(size);
}

void SpriteEditor::eraserSizeChanged()
{
    int size = ui->eraserSize->value();
    pencil->setEraserSize(size);
}

void SpriteEditor::eraserSelected()
{
    pencil->setToEraser();
    ui->eraserTool->setChecked(true);
    ui->penTool->setChecked(false);
}

void SpriteEditor::penSelected()
{
    pencil->setToPen();
    ui->eraserTool->setChecked(false);
    ui->penTool->setChecked(true);
}

void SpriteEditor::updatePenSizeLabel(int size)
{
    ui->penSizeLabel->setText("Pen Size: " + QString::number(size));
}

void SpriteEditor::updateEraserSizeLabel(int size)
{
    ui->eraserSizeLabel->setText("Eraser Size: " + QString::number(size));
}

void SpriteEditor::eyedropperToggled(bool checked)
{
    model->getFrame(currentFrameIndex)->setEyedropper(checked);
}

void SpriteEditor::dropperFinished()
{
    ui->eyedropperTool->setChecked(false);
}

void SpriteEditor::handleDropper()
{
    bool isActive = model->getFrame(currentFrameIndex)->getEyedropperEnabled();
    ui->eyedropperTool->setChecked(isActive);
}

void SpriteEditor::handleUndo()
{
    model->getFrame(currentFrameIndex)->undoPaint();
}

void SpriteEditor::handleRedo()
{
    model->getFrame(currentFrameIndex)->redoPaint();
}

void SpriteEditor::handleAdd()
{
    currentFrameIndex += 1;
    model->addFrame(currentFrameIndex);
    connect(model->getFrame(currentFrameIndex),
            &Sprite::changeColor,
            this,
            &SpriteEditor::updateColorSelector);
}
void SpriteEditor::handleCopy()
{
    model->copy(currentFrameIndex);
}

void SpriteEditor::handlePaste()
{
    model->paste(currentFrameIndex);
    updateFrameIcons();
    frameFocus(model->getFrame(currentFrameIndex + 1));
}
