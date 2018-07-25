#include "dialog.h"
#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    //创建一个用设计器设计的对话框对象
    Dialog w;
    w.show();

    return a.exec();
}
