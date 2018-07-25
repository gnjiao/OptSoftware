#ifndef DIALOG_H
#define DIALOG_H

#include <QDialog>

namespace Ui {
//放到Ui中以便和下面的Diabog类区分
class Dialog;
}

class Dialog : public QDialog
{
    Q_OBJECT

public:
    explicit Dialog(QWidget *parent = 0);
    ~Dialog();

private:
    Ui::Dialog *ui;
};

#endif // DIALOG_H
