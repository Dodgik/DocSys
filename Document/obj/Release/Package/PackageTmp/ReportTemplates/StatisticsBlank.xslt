<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text"/>
  <xsl:template match="/">
    <xsl:variable name="i" select="0"/>
    <xsl:variable name="colNum" select="0"/>
    
    <xsl:text>{\rtf1\ansi\ansicpg1251\deff0\deflang1049\deflangfe1049\deftab708{\fonttbl{\f0\fswiss\fprq2\fcharset204 Calibri;}{\f1\froman\fprq2\fcharset204{\*\fname Times New Roman;}Times New Roman CYR;}}</xsl:text>


    <xsl:text>\viewkind4\uc1\pard\qc\lang1058\f0\fs24</xsl:text>
    <xsl:for-each select="StatisticsBlank/Headers/string">
      <xsl:text> </xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>\par</xsl:text>
    </xsl:for-each>

    <xsl:text>\par</xsl:text>
    <xsl:text>\trowd\trgaph108\trleft-108\trrh365\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>

    <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7900</xsl:text>
    <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9575</xsl:text>
    <xsl:text>\pard\intbl\nowidctlpar\qc\fs24</xsl:text>
    <xsl:text>\cell</xsl:text>
    <xsl:text>\'ca\'b3\'eb\'fc\'ea\'b3\'f1\'f2\'fc</xsl:text>
    <xsl:text>\cell</xsl:text>
    <xsl:text>\row</xsl:text>

    <xsl:for-each select="StatisticsBlank/Statistics/ArrayOfString">

      <xsl:text>\trowd\trgaph108\trleft-108\trrh365\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
      
      <xsl:text>\clbrdrl\brdrw10\brdrs</xsl:text>

      <xsl:for-each select="string">
        <xsl:if test=".">
          <xsl:text>\clbrdrt\brdrw10\brdrs</xsl:text>
        </xsl:if>
        <xsl:text> </xsl:text>
        <xsl:value-of select="."/>
        <xsl:text>\par</xsl:text>
      </xsl:for-each>

      <xsl:for-each select="StatisticsBlank/ColumnsWidth/string">
        <xsl:if test=".">
          <xsl:text>\clbrdrt\brdrw10\brdrs</xsl:text>
        </xsl:if>
        <xsl:text> </xsl:text>
        <xsl:value-of select="."/>
        <xsl:text>\par</xsl:text>
      </xsl:for-each>

      <xsl:if test="Name">
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs</xsl:text>
        <xsl:if test="Number and Name">
          <xsl:text>\clbrdrr\brdrw10\brdrs</xsl:text>
        </xsl:if>
        <xsl:text>\clbrdrb\brdrw10\brdrs \cellx2218</xsl:text>
      </xsl:if>

      <xsl:if test="Count and Number">
        <xsl:text>\clbrdrl\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrt\brdrw10\brdrs</xsl:text>
      <xsl:if test="Count">
        <xsl:text>\clbrdrr\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrb\brdrw10\brdrs \cellx7900</xsl:text>

      <xsl:if test="Count">
        <xsl:text>\clbrdrl\brdrw10\brdrs</xsl:text>
      </xsl:if>
      <xsl:text>\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9575</xsl:text>

      <xsl:text>\pard\intbl\nowidctlpar </xsl:text>
      <xsl:value-of select="Number"/>
      <xsl:text>\cell </xsl:text>
      <xsl:if test="Name">
        <xsl:value-of select="Name"/>
        <xsl:text>\cell</xsl:text>
      </xsl:if>
      <xsl:text>\pard\intbl\nowidctlpar\qr </xsl:text>
      <xsl:value-of select="Count"/>
      <xsl:text>\cell</xsl:text>
      <xsl:text>\row</xsl:text>


      <xsl:text></xsl:text>
    </xsl:for-each>
    <xsl:text>\pard\sa200\sl276\slmult1\fs28</xsl:text>

    <xsl:text>\par</xsl:text>
    <xsl:text>\'c2\'e8\'ea\'ee\'ed\'e0\'e2 ______________________ \par</xsl:text>

    <xsl:text>}</xsl:text>

  </xsl:template>
</xsl:stylesheet>

<?xml version="1.0" encoding="utf-16"?>
<StatisticsBlank xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Headers>
    <string>ВІДОМОСТІ</string>
    <string>Про розгляд виконавцями звернень громадян за період</string>
    <string>З 2011.11.01 ПО 2011.11.18</string>
    <string>ВИКОНАВЕЦЬ: ХМИЛЕЦЬКИЙ О.В.</string>
  </Headers>
  <Statistics>
    <ArrayOfString>
      <string>Всього звернень:</string>
      <string>15</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Результати розгляду</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Первинне</string>
      <string xsi:nil="true" />
      <string>7</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Неодноразове</string>
      <string xsi:nil="true" />
      <string>8</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Індивідуальне</string>
      <string xsi:nil="true" />
      <string>11</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Колективне</string>
      <string xsi:nil="true" />
      <string>4</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Результати розгляду</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>НЕ ВИЗНАЧЕНО</string>
      <string xsi:nil="true" />
      <string>15</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Звернення, що надійшли через інші організації</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>АДМІНІСТРАЦІЯ ПРЕЗИДЕНТА УКРАЇНИ</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>ОБЛАСНА ДЕРЖАВНА АДМІНІСТРАЦІЯ</string>
      <string xsi:nil="true" />
      <string>4</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>ПРОКУРАТУРА М.РІВНЕ</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>ДУ "УРЯДОВИЙ КОНТАКТНИЙ ЦЕНТР"</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>ОДА - ГАРЯЧА ЛІНІЯ</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Категорії авторів звернень</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Учасник війни</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Ветеран праці</string>
      <string xsi:nil="true" />
      <string>2</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Інвалід ІІ групи</string>
      <string xsi:nil="true" />
      <string>1</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Інвалід ІІІ групи</string>
      <string xsi:nil="true" />
      <string>2</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Інші категорії</string>
      <string xsi:nil="true" />
      <string>9</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Питання, які порушені у зверненнях</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Аграрна політика, земельні відносини</string>
      <string xsi:nil="true" />
      <string>2</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Соціальний захист</string>
      <string xsi:nil="true" />
      <string>3</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Комунальне господарство</string>
      <string xsi:nil="true" />
      <string>4</string>
    </ArrayOfString>
    <ArrayOfString>
      <string xsi:nil="true" />
      <string>Житлова політика</string>
      <string xsi:nil="true" />
      <string>8</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Список громадян, які звернулися</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Артеменко Н.Г.</string>
      <string xsi:nil="true" />
      <string>відсутність опалення в будинку</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Богатько О.М.</string>
      <string xsi:nil="true" />
      <string>провести ремонт вулиці</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Вовк Є.В.</string>
      <string xsi:nil="true" />
      <string>не розформовувати ЖКП "Центральний"</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Вознюк О.Я.</string>
      <string xsi:nil="true" />
      <string>замінити газову плиту в гуртожитку</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Герасимчук О.</string>
      <string xsi:nil="true" />
      <string>облаштування асфальтного покриття вулиці</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Гуменюк Т.О.</string>
      <string xsi:nil="true" />
      <string>зрізати дерева перед вікнами та перерахунок послуг ЖКП</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Гуменюк Т.О.</string>
      <string xsi:nil="true" />
      <string>здійснення перерахунку за теплопостачання</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Демчук Н.І.</string>
      <string xsi:nil="true" />
      <string>
        відновити роботу сміттєпроводу
        не задоволена роботою ОСББ "Орбіта"
      </string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Курилюк С.Г.</string>
      <string xsi:nil="true" />
      <string>поліпшення житлових умов у гуртожитку</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Максимчук В.О.</string>
      <string xsi:nil="true" />
      <string>ремонт водопровідних мереж </string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Ребекевша С.В.</string>
      <string xsi:nil="true" />
      <string>відремонтувати під'їзд та дах будинку</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Семенюк А.І.</string>
      <string xsi:nil="true" />
      <string>надати інформацію щодо надання субсидій</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Серцов М.Я.</string>
      <string xsi:nil="true" />
      <string>підтоплюється підвальне приміщення</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Синьоока Л.І.</string>
      <string xsi:nil="true" />
      <string>ремонт водоровідних мереж у підвалі будинку</string>
      <string>На виконанні</string>
    </ArrayOfString>
    <ArrayOfString>
      <string>Шабалін С.С.</string>
      <string xsi:nil="true" />
      <string>Скарга на ЖКП "Паркове"</string>
      <string>На виконанні</string>
    </ArrayOfString>
  </Statistics>
</StatisticsBlank>