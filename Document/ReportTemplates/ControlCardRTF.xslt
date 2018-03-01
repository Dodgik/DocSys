<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

    <xsl:output method="text"/>
    
    <xsl:template match="/">
        <xsl:text>{\rtf1\ansi\ansicpg1251\deff0\deflang1058\deflangfe1058\deftab708{\fonttbl{\f0\froman\fprq2\fcharset204{\*\fname Times New Roman;}Times New Roman CYR;}{\f1\fswiss\fprq2\fcharset0 Calibri;}{\f2\fswiss\fprq2\fcharset204 Calibri;}}</xsl:text>

        <xsl:text>\viewkind4\uc1\pard\sa200\sl276\slmult1\qc\f0\fs20</xsl:text>
        <xsl:value-of select="ControlCardReport/DepartmentName"/>
        <xsl:text>\f1\fs24\par</xsl:text>
        <xsl:text>\trowd\trgaph108\trleft-108\trrh419\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx2127\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\lang1033\f1\fs20 </xsl:text>
        
        <xsl:text>\lang1058\f2\cell\pard\intbl\qc\b\'d0\'e5\'ba\'f1\'f2\'f0\'e0\'f6\'b3\'e9\'ed\'ee-\'ea\'ee\'ed\'f2\'f0\'ee\'eb\'fc\'ed\'e0 \'ea\'e0\'f0\'f2\'ea\'e0\b0\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/Control"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh425\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3969\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\qc\'ca\'ee\'f0\'e5\'f1\'ef\'ee\'ed\'e4\'e5\'ed\'f2\cell\'c0\'e4\'f0\'e5\'f1\'e0\cell\pard\intbl\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/InputDocType"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh402\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clvmgf\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3969\clvmgf\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\lang1033\f1\b </xsl:text>
        <xsl:value-of select="ControlCardReport/Correspondent"/>
        <xsl:text>\lang1058\f2\b0\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/Address"/>
        <xsl:text>\lang1058\f2\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/InputSubjectType"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh691\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clvmrg\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3969\clvmrg\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\cell\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/DeliveryType"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx1560\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3402\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx5457\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\qc\'c4\'e0\'f2\'e0 \'ed\'e0\'e4\'f5\'ee\'e4\'e6\'e5\'ed\'ed\'ff\cell\'d0\'e5\'ba\'f1\'f2\'f0\'e0\'f6\'b3\'e9\'ed\'e8\'e9 \'b3\'ed\'e4\'e5\'ea\'f1\cell\'c7\'e2\'b3\'e4\'ea\'e8 \'ee\'e4\'e5\'f0\'e6\'e0\'ed\'ee\cell\'c4\'e0\'f2\'e0 \'e2\'b3\'e4\'ef\'f0\'e0\'e2\'eb\'e5\'ed\'ed\'ff\cell\'b2\'ed\'e4\'e5\'ea\'f1 \'e4\'ee\'ea\'f3\'ec\'e5\'ed\'f2\'e0\cell\row\trowd\trgaph108\trleft-108\trrh769\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx1560\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3402\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx5457\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\qc\b\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/InternalCreationDate"/>
        <xsl:text>\lang1058\f2\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/InternalNumber"/>
        <xsl:text>\lang1058\f2\b0\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/OrganizationName"/>
        <xsl:text>\lang1058\f2\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/ExternalCreationDate"/>
        <xsl:text>\lang1058\f2\cell\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/ExternalNumber"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh567\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\'cf\'ee\'ef\'e5\'f0\'e5\'e4\'ed\'b3 \'e7\'e2\'e5\'f0\'ed\'e5\'ed\'ed\'ff \'e2\'b3\'e4\lang1033\f1  </xsl:text>
        
        <xsl:text>\lang1058\f2\cell\pard\intbl\qc\'cf\'e8\'f2\'e0\'ed\'ed\'ff/\'cf\'b3\'e4\'ef\'e8\'f2\'e0\'ed\'ed\'ff\cell\row\trowd\trgaph108\trleft-108\trrh1157\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\'ca\'ee\'f0\'ee\'f2\'ea\'e8\'e9 \'e7\'ec\'b3\'f1\'f2:\lang1033\f1  </xsl:text>
        <xsl:value-of select="ControlCardReport/Content"/>
        <xsl:text>\lang1058\f2\cell\pard\intbl\qc\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/Branches"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh929\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clvmgf\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\'d0\'e5\'e7\'ee\'eb\'fe\'f6\'b3\'ff:\lang1033\f1\b  </xsl:text>
        <xsl:value-of select="ControlCardReport/Resolution"/>
        <xsl:text>\lang1058\f2\b0\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\cell\pard\intbl\qc\'ca\'e0\'f2\'e5\'e3\'ee\'f0\'b3\'ff , \'f1\'ee\'f6\'b3\'e0\'eb\'fc\'ed\'e8\'e9 \'f1\'f2\'e0\'ed  \'e7\'e0\'ff\'e2\'ed\'e8\'ea\'e0\cell\row\trowd\trgaph108\trleft-108\trrh1130\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        <xsl:text>\clvmrg\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\cell\pard\intbl\qc\lang1033\f1 </xsl:text>
        <xsl:value-of select="ControlCardReport/SocialStatus"/>
        <xsl:text>, </xsl:text>
        <xsl:value-of select="ControlCardReport/SocialCategories"/>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh438\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\'c0\'e2\'f2\'ee\'f0 \'f0\'e5\'e7\'ee\'eb\'fe\'f6\'b3\'bf\lang1033\f1 :\lang1058\f2  \lang1033\f1\b </xsl:text>
        <xsl:value-of select="ControlCardReport/Head"/>

        <xsl:text>\lang1058\f2\b0\cell\pard\intbl\qc\lang1033\f1 </xsl:text>
        <xsl:text>\lang1058\f2\cell\row\trowd\trgaph108\trleft-108\trrh400\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7088\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\'d2\'e5\'f0\'ec\'b3\'ed \'e2\'e8\'ea\'ee\'ed\'e0\'ed\'ed\'ff\lang1033\f1 : \b </xsl:text>
        <xsl:value-of select="ControlCardReport/EndDate"/>
        <xsl:text>\lang1058\f2\b0\cell\pard\intbl\qc\'c2\'e8\'ea\'ee\'ed\'e0\'ed\'ee \'e7\'e0 ___ \'e4\'ed\'b3\'e2\cell\row\pard\sa200\sl276\slmult1\fs20\par</xsl:text>

        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        <xsl:text>\par</xsl:text>
        
        <xsl:text>\pard\sa200\sl276\slmult1\qc\'d5\'b3\'e4 \'e2\'e8\'ea\'ee\'ed\'e0\'ed\'ed\'ff\par</xsl:text>
        <xsl:text>\trowd\trgaph108\trleft-108\trrh966\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx1418\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3544\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7797\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\qc\fs20\'c4\'e0\'f2\'e0 \'ef\'e5\'f0\'e5\'e4\'e0\'f7\'b3 \'ed\'e0 \'e2\'e8\'ea\'ee\'ed\'e0\'ed\'ed\'ff\cell\'c2\'e8\'ea\'ee\'ed\'e0\'e2\'e5\'f6\'fc\cell\'c7\'e0\'ef\'e8\'f1\'e8 \'ef\'f0\'ee \'ef\'f0\'ee\'e4\'ee\'e2\'e6\'e5\'ed\'ed\'ff \'f2\'e5\'f0\'ec\'b3\'ed\'f3, \'ef\'ee\'ef\'e5\'f0\'e5\'e4\'ed\'fe \'e2\'b3\'e4\'ef\'ee\'e2\'b3\'e4\'fc \'e0\'e1\'ee \'e4\'ee\'e4\'e0\'f2\'ea\'ee\'e2\'b3 \'ef\'e8\'f2\'e0\'ed\'ed\'ff\cell\'ca\'ee\'ed\'f2\'f0\'ee\'eb\'fc\'ed\'b3 \'e2\'b3\'e4\'ec\'b3\'f2\'ea\'e8\cell\row\trowd\trgaph108\trleft-108\trrh5090\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl108\trpaddr108\trpaddfl3\trpaddfr3</xsl:text>
        <xsl:text>\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx1418\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx3544\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx7797\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx9498\pard\intbl\cell\cell\cell\cell\row\pard\sa200\sl276\slmult1\qc\fs20\par</xsl:text>
        <xsl:text>\pard\sa200\sl276\slmult1\'cf\'e5\'f0\'e5\'e2\'b3\'f0\'e5\'ed\'ee \'ed\'e0 \'ec\'b3\'f1\'f6\'b3 "__"_______ 20__\'f0. \'ef\'f0\'e0\'f6\'b3\'e2\'ed\'e8\'ea\'ee\'ec \'e0\'ef\'e0\'f0\'e0\'f2\'f3 ___\par</xsl:text>
        <xsl:text>\'c4\'e0\'f2\'e0, \'b3\'ed\'e4\'e5\'ea\'f1, \'e2\'e8\'ea\'ee\'ed\'e0\'ed\'ed\'ff ________\lang1033\f1 _\lang1058\f2 ___\lang1033\f1 __\lang1058\f2 __\lang1033\f1  \lang1058\f2\'c0\'e4\'f0\'e5\'f1\'e0\'f2\lang1033\f1   \lang1058\f2 ____________\lang1049\par</xsl:text>
        <xsl:text>\pard\ri417\sa200\sl276\slmult1\qr\tx7889\lang1058\'c2\'e8\'f0\'b3\'f8\'e5\'ed\'ee\lang1033\f1  \lang1049\f2 ____\lang1033\f1 _\lang1049\f2 _\lang1058 ______\par</xsl:text>
        <xsl:text>\pard\sa200\sl276\slmult1\lang1049 __________________________________________________________\lang1033\f1 _\lang1049\f2\par</xsl:text>
        <xsl:text>\lang1058\'d0\'e5\'e7\'f3\'eb\'fc\'f2\'e0\'f2\'e8 \'f0\'ee\'e7\'e3\'eb\'ff\'e4\'f3  \'ef\'e8\'f2\'e0\'ed\'fc:\lang1049  ________________________________\par</xsl:text>
        <xsl:text>__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________\par</xsl:text>
        <xsl:text>\lang1058\'c7 \'ea\'ee\'ed\'f2\'f0\'ee\'eb\'fe \'e7\'ed\'ff\'e2\lang1033\f1   \lang1058\f2 ___________________________________________\lang1049\par</xsl:text>
        <xsl:text>\lang1058\'d1\'ef\'f0\'e0\'e2\'e0_______________\'d2\'ee\'ec_______\'c0\'f0\'ea\'f3\'f8\'b3\'e2__________\'d4\'ee\'ed\'e4____\lang1049 _\par</xsl:text>
        <xsl:text>\pard\ri417\sa200\sl276\slmult1\qr\lang1058\'ce\'ef\'e8\'f1\lang1049  \lang1058 __\lang1049 __\lang1058 _________\lang1049  \lang1058\'d1\'ef\'f0\'e0\'e2\'e0____________\par</xsl:text>
        <xsl:text>\pard\sa200\sl276\slmult1\par</xsl:text>

        <xsl:text>}</xsl:text>

    </xsl:template>
</xsl:stylesheet>
