<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:mims="http://www.mims.com">
  <xsl:param name="D2DSeverityFilter">^2^3^4^5^</xsl:param>
  <xsl:param name="D2HSeverityFilter">^2^3^</xsl:param>
  <xsl:param name="DocumentationFilter">^1^2^3^4^</xsl:param>
  <xsl:param name="DuplicateTherapyFilter">^1^2^3^</xsl:param>
  <xsl:param name="DuplicateIngredientFilter">^1^2^3^</xsl:param>
  <xsl:param name="D2LSeverityFilter">^2^3^1^</xsl:param>
  <xsl:param name="CriticalPrecautionFilter">^Never use this combination.^Avoid combination.^Use combination with extreme caution.^Use combination with caution.^Clinical significance not established; be alert for evidence of interaction.^No action required; be alert for evidence of interaction.^Interaction not clinically significant.^</xsl:param>
  <xsl:output method="html" omit-xml-declaration="yes" indent="no"/>
  <!-- group allergy by prescribing item's reference and allergen reference -->
  <!--<xsl:key name="AllergyItems" match="//Allergy/*" use="concat(../../@reference, '-', @reference)"/>-->
  <xsl:key name="D2DItems" match="//Route/*[not(@Mirror='true' or @Mirror='True')]/Route/ClassInteraction" use="concat(PrescribingInteractionClass/@reference, '-', InteractingClass/@reference)"/>
  <xsl:key name="d2d_sev_level-lookup" match="mims:d2d_sev_level" use="@name"/>
  <xsl:key name="d2h_sev_level-lookup" match="mims:d2h_sev_level" use="@name"/>
  <xsl:key name="d2l_sev_level-lookup" match="mims:d2l_sev_level" use="@name"/>
  <xsl:key name="doc_level-lookup" match="mims:doc_level" use="@name"/>
  <xsl:key name="preg_cat-lookup" match="mims:preg_cat" use="@cat"/>
  <xsl:key name="preg_pic-lookup" match="mims:preg_pic" use="@cat"/>
  <xsl:variable name="d2d_sev_level-top" select="document('')/*/mims:d2d_sev_levels"/>
  <xsl:variable name="d2h_sev_level-top" select="document('')/*/mims:d2h_sev_levels"/>
  <xsl:variable name="d2l_sev_level-top" select="document('')/*/mims:d2l_sev_levels"/>
  <xsl:variable name="doc_level-top" select="document('')/*/mims:doc_levels"/>
  <xsl:variable name="preg_cat-top" select="document('')/*/mims:preg_cats"/>
  <xsl:variable name="preg_pic-top" select="document('')/*/mims:preg_pics"/>
  <xsl:variable name="criticalPrecaution">./Precaution[Professional='Use combination with extreme caution.']/Professional</xsl:variable>
  <xsl:template match="/">
    <html>
      <head>
				<link type="text/css" href="CSS/redmond/jquery-ui.css" rel="stylesheet" />
				<link type="text/css" href="CSS/mims.css" rel="stylesheet" />
				<link type="text/css" href="CSS/logoandstatement.css" rel="stylesheet" />
				<script type="text/javascript" src="Scripts/jquery-3.6.0.min.js"/>
				<script type="text/javascript" src="Scripts/jquery-ui.min.js"/>
				<script type="text/javascript" src="Scripts/jquery-migrate-3.3.2.min.js"/>
				<xsl:choose>
					<xsl:when test="count(//Interaction) &gt; 0">
						<script type="text/javascript" src="Scripts/drugalert.js"/>
					</xsl:when>
					<xsl:otherwise>
						<script type="text/javascript" src="Scripts/monograph.js"/>
					</xsl:otherwise>
				</xsl:choose>
        <style>div.drug-label {	width: 90mm;	height: 35mm; border: 1pt solid; border-radius: 5pt;	padding: 5pt;	margin-bottom: 5pt;	}	.drug-label h4 {	font-size: 10pt;	font-weight: bold;	font-family: Arial;	margin: 2pt;	}	.drug-label p {	font-size: 8pt;	font-weight: normal;	font-family: Arial;	margin: -1pt 2pt 2pt 2pt;	}	.cals-details p {	margin-bottom: 0pt;	}	.cal-details li a {	text-decoration: none;	}	.cal-details strong { margin-right: 2pt; }	ol.allergy-list { padding-left: 1.33em; }	ol.allergy-list > li { padding-top: 0.5em; }	ol.allergy-list h4.subheading { margin-top: 0.2em; }</style>
      </head>
      <body>
        <xsl:apply-templates select="Result"/>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="Result">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="List">
    <xsl:if test="count(./*) &gt; 0">
      <xsl:if test="count(Product) &gt; 0">
        <xsl:call-template name="listProduct">
          <xsl:with-param name="Nodes" select="Product"/>
        </xsl:call-template>
      </xsl:if>
      <xsl:if test="count(GGPI|GenericItem|SpecificItem|ActiveCompositionGroup|ActiveComposition|SubstanceClass|HealthIssue) &gt; 0">
        <xsl:call-template name="listItem">
          <xsl:with-param name="Nodes" select="GGPI|GenericItem|SpecificItem|ActiveCompositionGroup|ActiveComposition|SubstanceClass|HealthIssue"/>
        </xsl:call-template>
      </xsl:if>
      <xsl:if test="count(HealthIssueCode) &gt; 0">
        <xsl:call-template name="listHealthCode">
          <xsl:with-param name="Nodes" select="HealthIssueCode"/>
        </xsl:call-template>
      </xsl:if>
    </xsl:if>
  </xsl:template>
  <xsl:template name="listProduct">
    <xsl:param name="Nodes"/>
    <table border="0px" cell-padding="0px">
      <tr>
        <th class="interaction" width="55%">Product</th>
        <th class="interaction" width="30%">GUID</th>
        <th class="interaction" width="15%">OffMarket</th>
      </tr>
      <xsl:for-each select="*">
        <tr>
          <td class="interaction" width="55%">
            <xsl:value-of select="@name"/>
          </td>
          <td class="interaction" width="30%">
            <xsl:value-of select="@reference"/>
          </td>
          <td class="interaction" width="15%">
            <xsl:value-of select="@offMarket"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="listItem">
    <xsl:param name="Nodes"/>
    <table border="0px" cell-padding="0px">
      <tr>
        <th class="interaction" width="60%">Name</th>
        <th class="interaction" width="20%">GUID</th>
      </tr>
      <xsl:for-each select="*">
        <tr>
          <td class="interaction" width="60%">
            <xsl:value-of select="@name"/>
          </td>
          <td class="interaction" width="20%">
            <xsl:value-of select="@reference"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template name="listHealthCode">
    <xsl:param name="Nodes"/>
    <table border="0px" cell-padding="0px">
      <tr>
        <th class="interaction" width="20%">Code Type</th>
        <th class="interaction" width="20%">Code</th>
        <th class="interaction" width="60%">Name</th>
      </tr>
      <xsl:for-each select="*">
        <tr>
          <td class="interaction" width="20%">
            <xsl:value-of select="@codeType"/>
          </td>
          <td class="interaction" width="20%">
            <xsl:value-of select="@code"/>
          </td>
          <td class="interaction" width="60%">
            <xsl:value-of select="@name"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
  <xsl:template match="Interaction">
    <xsl:variable name="numDT" select="count(//DuplicateTherapy/Warning[contains($DuplicateTherapyFilter, concat('^',@Level, '^'))]/Duplicate)"/>
    <xsl:variable name="numHealth" select="count(//HealthIssue/ClassInteraction[contains($D2HSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))]) + count(//HealthIssueCode/ClassInteraction[contains($D2HSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))])"/>
    <!-- Allergy, only count prescribing items that have at least 1 allergy. Not the total number of allergies -->
    <xsl:variable name="numAllergy" select="count(//*[Allergy])"/>
    <xsl:variable name="numInteract" select="count(//Route/*[not(@Mirror='true' or @Mirror='True')]/Route/ClassInteraction[contains($D2DSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))])"/>
    <xsl:variable name="numDose" select="count(//DoseCheck/*/Warnings[count(*) &gt; 0]) + count(//CombinedDoseCheck/*/Warnings[count(*) &gt; 0])"/>
    <!--<xsl:variable name="numPreg" select="count(//*[Pregnancy/InteractionClass]) + count(//*[WOCBA/InteractionClass])"/>-->
    <xsl:variable name="numPreg" select="count(//*[Pregnancy/Category]) + count(//*[WOCBA/Category])"/>
    <!--<xsl:variable name="numLact" select="count(//Lactation/InteractionClass[count(*) &gt; 0])"/>
		<xsl:variable name="numLact" select="count(//Lactation/InteractionClass[count(*) &gt; 0])"/>-->
    <xsl:variable name="numLact" select="count(//*[Lactation])"/>

    <xsl:variable name="numDupIng" select="count(//DuplicateIngredient/Warning[contains($DuplicateIngredientFilter, concat('^',@Level, '^'))]/Molecule)"/>
    <xsl:variable name="numLabels" select="count(//*[CautionaryLabels])"/>
    <!--VN Interactions count-->
    <xsl:variable name="numvn" select="count(//DANH_SACH_TUONG_TAC/CAP_TUONG_TAC)"/>
    <xsl:variable name="numTotal" select="$numDT + $numHealth + $numvn + $numAllergy + $numInteract + $numDose + $numPreg + $numLact + $numLabels + $numDupIng"/>
    <!-- Only count number of drugs that has dose alerts -->
    <xsl:choose>
      <xsl:when test="$numTotal &gt; 0">
        <div id="tabs" class="tab-container">
          <ul>
		  <div class="container1">
  <img src="images/MIMSIntegratedfinallogo.png" alt="Logo" style="width:150px; height:50px; cursor:pointer;" />
  <span>
    <xsl:value-of select="//Result/@copyright"/>
    <xsl:text> | Database version/BuildID: </xsl:text>
    <xsl:value-of select="//Result/@buildID"/>
    <xsl:text> | Expiry date: </xsl:text>
    <xsl:value-of select="//Result/@expiryDate"/>
  </span>
</div>
            <xsl:if test="$numInteract &gt; 0">
              <li>
                <a href="#tab-interaction">
                  Tương tác thuốc (<xsl:value-of select="$numInteract"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numAllergy &gt; 0">
              <li>
                <a href="#tab-allergy">
                  Dị ứng (<xsl:value-of select="$numAllergy"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numHealth &gt; 0">
              <li>
                <a href="#tab-health">
                  Tình trạng sức khỏe (<xsl:value-of select="$numHealth"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numDT &gt; 0">
              <li>
                <a href="#tab-dup">
                  Trùng lặp liệu pháp (<xsl:value-of select="$numDT"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numDupIng &gt; 0">
              <li>
                <a href="#tab-duping">
                  Trùng lặp thành phần (<xsl:value-of select="$numDupIng"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numDose &gt; 0">
              <li>
                <a href="#tab-dose">
                  Liều (<xsl:value-of select="$numDose"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numPreg &gt; 0">
              <li>
                <a href="#tab-preg">
                  Thai kỳ (<xsl:value-of select="$numPreg"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numLact &gt; 0">
              <li>
                <a href="#tab-lact">
                  Cho con bú (<xsl:value-of select="$numLact"/>)
                </a>
              </li>
            </xsl:if>
            <!--For VN Interactions-->
            <xsl:if test="$numvn &gt; 0">
              <li>
                <a href="#tab-vn">
                  Tương tác chống chỉ định VN  (<xsl:value-of select="$numvn"/>)
                </a>
              </li>
            </xsl:if>
            <xsl:if test="$numLabels &gt; 0">
              <li>
                <a href="#tab-labels">
                  Nhãn cảnh báo (<xsl:value-of select="$numLabels"/>)
                </a>
              </li>
            </xsl:if>
          </ul>
          <xsl:if test="$numDT &gt; 0">
            <div id="tab-dup" class="acc_container">
              <div id="list_dup">
                <xsl:apply-templates select="DuplicateTherapy"/>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numDupIng &gt; 0">
            <div id="tab-duping" class="acc_container">
              <div id="list_duping">
                <xsl:apply-templates select="DuplicateIngredient"/>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numInteract &gt; 0">
            <div id="tab-interaction" class="acc_container">
              <div id="list_interactions">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)">
                  <xsl:with-param name="Mode" select="'drug'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numHealth &gt; 0">
            <div id="tab-health" class="acc_container">
              <div id="list_health">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)">
                  <xsl:with-param name="Mode" select="'health'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <!--For VN Interactions-->
          <xsl:if test="$numvn &gt; 0">
            <div id="tab-vn" class="acc_container">
              <div id="list_vn">
                <xsl:apply-templates select="(DANH_SACH_TUONG_TAC)">
                  <xsl:with-param name="Mode" select="'vn'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numAllergy &gt; 0">
            <div id="tab-allergy" class="acc_container">
              <div id="list_allergies">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)">
                  <xsl:with-param name="Mode" select="'allergy'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numDose &gt; 0">
            <div id="tab-dose" class="acc_container">
              <div id="list_dose">
                <xsl:apply-templates select="DoseCheck|CombinedDoseCheck"/>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numPreg &gt; 0">
            <div id="tab-preg" class="acc_container">
              <div id="list_preg">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)">
                  <xsl:with-param name="Mode" select="'pregnancy'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numLact &gt; 0">
            <div id="tab-lact" class="acc_container">
              <div id="list_lact">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)">
                  <xsl:with-param name="Mode" select="'lactation'"/>
                </xsl:apply-templates>
              </div>
            </div>
          </xsl:if>
          <xsl:if test="$numLabels &gt; 0">
            <div id="tab-labels" class="acc_container">
              <div id="list_labels">
                <xsl:apply-templates select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)[CautionaryLabels]" mode="label"/>
              </div>
            </div>
          </xsl:if>
        </div>
      </xsl:when>
      <xsl:otherwise>
        <h3 class="ui-state-default ui-widget-content ui-widget-header" style="height:30px;text-align:center;padding-top: 10px">Không tìm thấy. Không tìm thấy tương tác không nên được hiểu là an toàn. Nên thực hiện đánh giá lâm sàng.</h3>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup">
    <xsl:param name="Mode"/>
    <xsl:variable name="IntType1" select="name()"/>
    <xsl:variable name="IntProd1" select="@name"/>
    <xsl:variable name="IntRef1" select="@reference"/>
    <xsl:choose>
      <xsl:when test="$Mode='allergy' and Allergy">
        <xsl:element name="div">
          <xsl:variable name="DivId">
            a_<xsl:value-of select="translate(@name, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
          </xsl:variable>
          <xsl:attribute name="id">
            <xsl:value-of select="$DivId"/>
          </xsl:attribute>
          <h3>
            <a href="#">
              Patient may be allergic to the prescribing item, <xsl:value-of select="$IntProd1"/>.
            </a>
          </h3>
          <div>
            <!--<xsl:for-each select="Allergy/*[count(. | key('AllergyItems', concat($IntRef1, '-', @reference))[1])=1]">-->
            <xsl:apply-templates select="Allergy">
              <xsl:with-param name="IntProd1" select="$IntProd1"/>
            </xsl:apply-templates>
            <!--</xsl:for-each>-->
          </div>
        </xsl:element>
      </xsl:when>
      <!--d2p-->

      <xsl:otherwise>
        <xsl:for-each select="Route">
          <xsl:variable name="IntRoute1" select="@name"/>

          <xsl:if test="$Mode='pregnancy' and Pregnancy|WOCBA">
            <xsl:variable name="DivId">
              <xsl:value-of select="translate($IntProd1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
            </xsl:variable>
            <!--concate category with description-->

            <xsl:for-each select="WOCBA|Pregnancy">
              <xsl:sort select="concat(Category[@Source='FDA']/@name,'-',InteractionClass/Molecule/@name)" order="descending"/>

              <!--<xsl:choose>-->
              <!--//Category[@name='B' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]-->
              <!--first loop B with +-->
              <xsl:for-each select="./Category[@name='B' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->
                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->

                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'B', '4'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'B', '4')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>

                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--second loop B without + -->
              <!--<xsl:for-each select="./Category[@name='B' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]">-->
              <xsl:for-each select="./Category[@name='B' and not(following-sibling::Category[@name='+'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'B', '2')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>

                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <!--
											<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="//Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>
											-->
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--</xsl:choose>-->
              <!--third loop  only '+' -->
              <xsl:for-each select="./Category[@name='+'][not(preceding-sibling::Category[@name='B']) and not(preceding-sibling::Category[@name='X']) and not(preceding-sibling::Category[@name='C'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, '+', '4')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>

                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <!--
											<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->

                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--fourth loop X with + will be red Only-->
              <xsl:for-each select="./Category[@name='X' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'X', '5')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--5th loop with X only no '+'-->
              <xsl:for-each select="./Category[@name='X'][not(preceding-sibling::Category[@name='+']) and not(following-sibling::Category[@name='+'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'X', '5')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--6th with C only not '+'-->
              <xsl:for-each select="./Category[@name='C'][not(preceding-sibling::Category[@name='+']) and not(following-sibling::Category[@name='+'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'C', '3')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>

              <!--7th with D only not '+'-->
              <xsl:for-each select="./Category[@name='D'][not(preceding-sibling::Category[@name='+']) and not(following-sibling::Category[@name='+'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'D', '4')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--	<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--8th loop C with + will be orange Only-->
              <xsl:for-each select="./Category[@name='C' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'C', '4'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'C', '4')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--	<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--</xsl:choose>-->
              <!--9th with A only not '+'-->
              <xsl:for-each select="./Category[@name='A'][not(preceding-sibling::Category[@name='+']) and not(following-sibling::Category[@name='+'])]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name,'XD+CBA', '544321'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'A', '6')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--</xsl:choose>-->
              <!--10th loop A with + will be orange Only-->
              <xsl:for-each select="./Category[@name='A' and @Source='FDA' and following-sibling::Category[@name='+' and @Source='MIMS']]">
                <!--<xsl:sort select="InteractionClass/Molecule/@name" order="ascending"/>-->
                <!-- Take the most severe item by FDA preg cat -->

                <!--<xsl:sort select="translate(Category/@name, 'XDCBA+', '543263')" order="descending"/>-->
                <xsl:if test="position()=1">
                  <!-- Only loop once -->
                  <xsl:element name="div">
                    <xsl:attribute name="id">
                      <xsl:value-of select="concat('pc_',translate(./@name, 'A', '4'),'_',$DivId, '_pc')"/>
                    </xsl:attribute>
                    <h3>
                      <a href="#">
                        <xsl:call-template name="SeverityColor">
                          <xsl:with-param name="SeverityLevel" select="translate(./@name, 'A', '4')"/>
                        </xsl:call-template>
                        <xsl:value-of select="$IntProd1"/>
                        <xsl:text> (</xsl:text>
                        <xsl:value-of select="../InteractionClass/Molecule/@name"/>
                        <xsl:text>/</xsl:text>
                        <xsl:value-of select="../../@name"/>
                        <xsl:text>) [</xsl:text>
                        <!--<xsl:choose>
												<xsl:when test="name()='WOCBA'">
													<xsl:text>Women of Childbearing Age</xsl:text>
												</xsl:when>
												<xsl:when test="name()='Pregnancy'">
													<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>
													<xsl:text> Trimester</xsl:text>
												</xsl:when>
											</xsl:choose>-->
                        <xsl:choose>

                          <!--test="name()='WOCBA'"-->
                          <xsl:when test="//Route[starts-with(local-name(WOCBA), 'WOCBA')]">
                            <xsl:text>Women of Childbearing Age</xsl:text>
                          </xsl:when>
                          <xsl:when test="//Route[starts-with(local-name(Pregnancy), 'Pregnancy')]">
                            <!--<xsl:value-of select="./Pregnancy[position()=1]/Category/@Trimester"/>-->
                            <xsl:value-of select="@Trimester"/>
                            <xsl:text> Trimester</xsl:text>
                          </xsl:when>
                        </xsl:choose>
                        <xsl:text>]</xsl:text>
                      </a>
                    </h3>
                    <div>
                      <xsl:apply-templates select=".."/>
                    </div>
                  </xsl:element>
                </xsl:if>
              </xsl:for-each>
              <!--</xsl:choose>-->
            </xsl:for-each>
          </xsl:if>

          <xsl:if test="$Mode='lactation'">
            <xsl:for-each select="Lactation">
              <xsl:apply-templates select=".">
                <xsl:with-param name="IntProd" select="$IntProd1"/>
                <xsl:with-param name="Route" select="../@name"/>
              </xsl:apply-templates>
            </xsl:for-each>
          </xsl:if>
          <xsl:if test="$Mode='drug'">
            <xsl:for-each select="(DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup)[not(@Mirror='true' or @Mirror='True')]">
              <xsl:variable name="IntType2" select="name()"/>
              <xsl:apply-templates select="Route">
                <xsl:with-param name="IntProd1" select="$IntProd1"/>
                <xsl:with-param name="IntProd2" select="@name"/>
                <xsl:with-param name="IntRoute1" select="$IntRoute1"/>
              </xsl:apply-templates>
            </xsl:for-each>
          </xsl:if>
          <xsl:if test="$Mode='health'">
            <xsl:apply-templates select="HealthIssueCode">
              <xsl:with-param name="IntProd1" select="$IntProd1"/>
              <xsl:with-param name="IntRoute1" select="$IntRoute1"/>
            </xsl:apply-templates>
            <xsl:apply-templates select="HealthIssue">
              <xsl:with-param name="IntProd1" select="$IntProd1"/>
              <xsl:with-param name="IntRoute1" select="$IntRoute1"/>
            </xsl:apply-templates>
          </xsl:if>
        </xsl:for-each>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template match="Route">
    <xsl:param name="IntProd1"/>
    <xsl:param name="IntProd2"/>
    <xsl:param name="IntRoute1"/>
    <xsl:param name="Mode"/>
    <xsl:variable name="IntRoute2" select="@name"/>
    <xsl:for-each select="ClassInteraction[contains($D2DSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))]">
      <xsl:variable name="IntMol1" select="PrescribingInteractionClass/PrescribingMolecule/@name"/>
      <xsl:variable name="IntMol2" select="InteractionClass/Molecule/@name"/>
      <xsl:variable name="DivId">
        i_<xsl:value-of select="Severity/@ranking"/>_<xsl:value-of select="translate($IntMol1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>_<xsl:value-of select="translate($IntMol2, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>
      </xsl:variable>
      <xsl:element name="div">
        <xsl:attribute name="id">
          <xsl:value-of select="$DivId"/>
        </xsl:attribute>
        <h3>
          <a href="#">
            <xsl:call-template name="SeverityColor">
              <xsl:with-param name="SeverityLevel" select="Severity/@ranking"/>
            </xsl:call-template>
            <!-- Leo - Show critical precaution -->
            <xsl:value-of select="$IntProd1"/> (<xsl:value-of select="$IntMol1"/>/<xsl:value-of select="$IntRoute1"/>)<xsl:text> vs </xsl:text>
            <xsl:value-of select="$IntProd2"/> (<xsl:value-of select="$IntMol2"/>/<xsl:value-of select="$IntRoute2"/>)<xsl:call-template name="ShowCriticalPrecaution">
              <xsl:with-param name="PrecautionText" select="./Precaution[contains($CriticalPrecautionFilter,concat('^',Professional/text(),'^'))]/Professional"/>
            </xsl:call-template>
          </a>
        </h3>
        <div>
          <h4 class="subheading">Hậu quả tương tác</h4>
          <p>
            <xsl:value-of select="$IntMol1"/>
            <xsl:text> </xsl:text>
            <xsl:choose>
              <xsl:when test="normalize-space(Observation/Abbreviated)">
                <xsl:value-of select="Observation/Abbreviated"/>
              </xsl:when>
              <xsl:when test="normalize-space(Observation/Professional)">
                <xsl:value-of select="Observation/Professional"/>
              </xsl:when>
            </xsl:choose>
            <xsl:text> </xsl:text>
            <xsl:value-of select="$IntMol2"/>
          </p>
          <xsl:call-template name="d2dSeverity">
            <xsl:with-param name="Severity" select="Severity/@ranking"/>
          </xsl:call-template>
          <xsl:call-template name="docLevel">
            <xsl:with-param name="DocLevel" select="Documentation/@ranking"/>
          </xsl:call-template>
          <!-- Show the rest of content in hidden div <xsl:element name="button"><xsl:attribute name="name">_ref</xsl:attribute><xsl:attribute name="id">_br_<xsl:value-of select="$DivId"/></xsl:attribute><xsl:attribute name="class">read-more</xsl:attribute><xsl:text>Read more </xsl:text></xsl:element>-->
          <xsl:element name="div">
            <xsl:attribute name="id">
              r_<xsl:value-of select="$DivId"/>
            </xsl:attribute>
            <xsl:attribute name="style">display:block</xsl:attribute>
            <h4 class="subheading">Cơ chế</h4>
            <xsl:choose>
              <xsl:when test="normalize-space(Interaction/Abbreviated)">
                <xsl:value-of select="Interaction/Abbreviated"/>
              </xsl:when>
              <xsl:when test="normalize-space(Interaction/Professional)">
                <xsl:value-of select="Interaction/Professional"/>
              </xsl:when>
            </xsl:choose>
            <h4 class="subheading">Cách xử trí</h4>
            <ol>
              <xsl:apply-templates select="Precaution"/>
            </ol>
            <xsl:apply-templates select="References">
              <xsl:with-param name="DivId" select="$DivId"/>
            </xsl:apply-templates>
          </xsl:element>
        </div>
      </xsl:element>
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="ShowCriticalPrecaution">
    <xsl:param name="PrecautionText"/>
    <xsl:if test="normalize-space($PrecautionText)">
      <xsl:text> [</xsl:text>
      <xsl:value-of select="$PrecautionText"/>
      <xsl:text>]</xsl:text>
    </xsl:if>
  </xsl:template>
  <xsl:template match="WOCBA|Pregnancy">
    <xsl:variable name="PCAT" select="Category[@Source='FDA']/@name"/>
    <xsl:if test="normalize-space($PCAT)">
      <h4 class="subheading">
        Phân loại FDA<xsl:text> </xsl:text>
        <xsl:element name="img">
          <xsl:attribute name="src">
            <xsl:apply-templates select="$preg_pic-top">
              <xsl:with-param name="Source" select="'FDA'"/>
              <xsl:with-param name="Cat" select="$PCAT"/>
            </xsl:apply-templates>
          </xsl:attribute>
        </xsl:element>
      </h4>
      <p>
        <xsl:apply-templates select="$preg_cat-top">
          <xsl:with-param name="Source" select="'FDA'"/>
          <xsl:with-param name="Cat" select="$PCAT"/>
        </xsl:apply-templates>
      </p>
    </xsl:if>
    <xsl:if test="count(Category[@Source='MIMS']) &gt; 0">
      <h4 class="subheading">Ghi chú của MIMS</h4>
      <p>
        <xsl:value-of select="Category[@Source='MIMS']/Comment"/>
      </p>
    </xsl:if>
  </xsl:template>
  <xsl:template match="Lactation">
    <xsl:param name="IntProd"/>
    <xsl:param name="Route"/>
    <!--<xsl:variable name="DivId">la_<xsl:value-of select="translate($IntProd, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
		</xsl:variable>-->
    <xsl:variable name="DivId">
      la_<xsl:value-of select="Severity/@ranking"/>_<xsl:value-of select="translate($IntProd, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>_la
    </xsl:variable>
    <xsl:element name="div">
      <xsl:attribute name="id">
        <xsl:value-of select="$DivId"/>
      </xsl:attribute>
      <h3>
        <a href="#">
          <xsl:call-template name="SeverityColor">
            <xsl:with-param name="SeverityLevel" select="translate(Severity/@ranking, '123', '543')"/>
          </xsl:call-template>
          <xsl:value-of select="$IntProd"/>
        </a>
      </h3>
      <div>
        <xsl:call-template name="d2lSeverity">
          <xsl:with-param name="Severity" select="Severity/@ranking"/>
        </xsl:call-template>
        <p>
          <label style="font-size:12px;font-weight:bold;margin-up:-410px">
            Mô tả<br/>
          </label>
          <xsl:value-of select="Comment"/>
        </p>
        <xsl:apply-templates select="References"/>
      </div>
    </xsl:element>
  </xsl:template>
  <xsl:template match="HealthIssueCode">
    <xsl:param name="IntProd1"/>
    <xsl:param name="IntRoute1"/>
    <xsl:variable name="IntHealthCode" select="@code"/>
    <xsl:variable name="IntHealthCodeType" select="@codeType"/>
    <xsl:variable name="IntHealthCodeDesc" select="@name"/>
    <xsl:for-each select="ClassInteraction[contains($D2HSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))]">
      <xsl:variable name="IntMol1" select="PrescribingInteractionClass/PrescribingMolecule/@name"/>
      <xsl:variable name="DivId">
        i_<xsl:value-of select="Severity/@ranking"/>_<xsl:value-of select="translate($IntMol1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>_<xsl:value-of select="translate($IntHealthCode, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>
      </xsl:variable>
      <xsl:element name="div">
        <xsl:attribute name="id">
          <xsl:value-of select="$DivId"/>
        </xsl:attribute>
        <h3>
          <a href="#">
            <xsl:call-template name="SeverityColor">
              <!-- Health has less severity levels, so we need to map it to our 5 colors -->
              <xsl:with-param name="SeverityLevel" select="translate(Severity/@ranking, '123', '135')"/>
            </xsl:call-template>
            <xsl:value-of select="$IntProd1"/> (<xsl:value-of select="$IntMol1"/>)<xsl:text> vs </xsl:text>
            <xsl:value-of select="$IntHealthCode"/> (<xsl:value-of select="$IntHealthCodeDesc"/>)
          </a>
        </h3>
        <div>
          <xsl:call-template name="d2hSeverity">
            <xsl:with-param name="Severity" select="Severity/@ranking"/>
          </xsl:call-template>
          <xsl:call-template name="docLevel">
            <xsl:with-param name="DocLevel" select="Documentation/@ranking"/>
          </xsl:call-template>
          <!-- Show the rest of content in hidden div<xsl:element name="button"><xsl:attribute name="name">_ref</xsl:attribute><xsl:attribute name="id">_br_<xsl:value-of select="$DivId"/></xsl:attribute><xsl:attribute name="class">read-more</xsl:attribute><xsl:text>Read more</xsl:text></xsl:element> -->
          <xsl:element name="div">
            <xsl:attribute name="id">
              r_<xsl:value-of select="$DivId"/>
            </xsl:attribute>
            <xsl:attribute name="style">display: block</xsl:attribute>
            <h4 class="subheading">Probable Mechanism</h4>
            <xsl:choose>
              <xsl:when test="normalize-space(Interaction/Abbreviated)">
                <xsl:value-of select="Interaction/Abbreviated"/>
              </xsl:when>
              <xsl:when test="normalize-space(Interaction/Professional)">
                <xsl:value-of select="Interaction/Professional"/>
              </xsl:when>
            </xsl:choose>
            <xsl:apply-templates select="References">
              <xsl:with-param name="DivId" select="$DivId"/>
            </xsl:apply-templates>
          </xsl:element>
        </div>
      </xsl:element>
    </xsl:for-each>
  </xsl:template>
  <!--For VN Interactions-->
  <xsl:template match="//Interaction/DANH_SACH_TUONG_TAC">
    <xsl:for-each select="//DANH_SACH_TUONG_TAC/CAP_TUONG_TAC">
      <xsl:variable name="Int1" select="HoatChat_1"/>
      <xsl:variable name="Int2" select="HoatChat_2"/>
      <xsl:variable name="Int3" select="MucDoNghiemTrong"/>
      <xsl:element name="div">
        <h3>
          <a href="#">
            <xsl:if test="MucDoNghiemTrong ='Chống chỉ định có điều kiện'">
              <img src="css/redmond/images/VN-level1.jpg" width='15px' height='15px'/> <!--orange-->
              &#160;<xsl:value-of select="$Int1"/>
              <xsl:text> vs </xsl:text>
              <xsl:value-of select="$Int2"/> (<xsl:value-of select="$Int3"/>)
            </xsl:if>
            <xsl:if test="MucDoNghiemTrong ='Chống chỉ định'">
              <img src="css/redmond/images/VN-level2.jpg" width='15px' height='15px'/> <!--red-->
              &#160;<xsl:value-of select="$Int1"/>
              <xsl:text> vs </xsl:text>
              <xsl:value-of select="$Int2"/> (<xsl:value-of select="$Int3"/>)
            </xsl:if>
          </a>
        </h3>
        <div>
          <h4 class="subheading">Mức độ tương tác</h4>
          <xsl:value-of select="MucDoNghiemTrong"/>
          <br/>
          <h4 class="subheading">Cơ chế tương tác</h4>
          <xsl:value-of select="CoCheTuongTac"/>
          <br/>

          <h4 class="subheading">Hậu quả tương tác</h4>
          <xsl:value-of select="HauQuaCuaTuongTac"/>
          <br/>

          <h4 class="subheading">Xử trí tương tác</h4>
          <xsl:value-of select="XuTriTuongTac"/>
          <!-- reference added feb-9-2024-->
          <h4 class="subheading">Tài Liệu Tham Khảo:</h4>
          <xsl:value-of select="TaiLieuThamKhao"/>
          <br/>
          <br/>
          <br/>
          <!-- disclaimer added feb-29-2024-->
          <h4 class="subheading" style="font-size: 10px;color:red;font-style: italic;">Tuyên Bố Miễn Trừ Trách Nhiệm:</h4>
          <p style="font-size: 9px;font-style: italic;">
            <xsl:value-of select="TuyenBoMienTruTrachNhiem"/>
          </p>

        </div>
      </xsl:element>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template match="HealthIssue">
    <xsl:param name="IntProd1"/>
    <xsl:param name="IntRoute1"/>
    <xsl:variable name="IntHealthIssueName" select="@name"/>
    <xsl:variable name="IntHealthCodeType" select="@codeType"/>
    <xsl:for-each select="ClassInteraction[contains($D2HSeverityFilter, concat('^',Severity/@ranking, '^')) or contains($DocumentationFilter, concat('^',Documentation/@ranking, '^'))]">
      <xsl:variable name="IntMol1" select="PrescribingInteractionClass/PrescribingMolecule/@name"/>
      <xsl:variable name="DivId">
        i_<xsl:value-of select="Severity/@ranking"/>_<xsl:value-of select="translate($IntMol1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>_<xsl:value-of select="translate($IntHealthIssueName, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ .','abcdefghijklmnopqrstuvwxyz__')"/>
      </xsl:variable>
      <xsl:element name="div">
        <xsl:attribute name="id">
          <xsl:value-of select="$DivId"/>
        </xsl:attribute>
        <h3>
          <a href="#">
            <xsl:call-template name="SeverityColor">
              <xsl:with-param name="SeverityLevel" select="translate(Severity/@ranking, '123', '135')"/>
            </xsl:call-template>
            <xsl:value-of select="$IntProd1"/> (<xsl:value-of select="$IntMol1"/>)<xsl:text> vs </xsl:text>
            <xsl:value-of select="$IntHealthIssueName"/>
          </a>
        </h3>
        <div>
          <xsl:call-template name="d2hSeverity">
            <xsl:with-param name="Severity" select="Severity/@ranking"/>
          </xsl:call-template>
          <xsl:call-template name="docLevel">
            <xsl:with-param name="DocLevel" select="Documentation/@ranking"/>
          </xsl:call-template>
          <!-- Show the rest of content in hidden div <xsl:element name="button"><xsl:attribute name="name">_ref</xsl:attribute><xsl:attribute name="id">_br_<xsl:value-of select="$DivId"/></xsl:attribute><xsl:attribute name="class">read-more</xsl:attribute><xsl:text>Read more</xsl:text></xsl:element>-->
          <xsl:element name="div">
            <xsl:attribute name="id">
              r_<xsl:value-of select="$DivId"/>
            </xsl:attribute>
            <xsl:attribute name="style">display: block</xsl:attribute>
            <h4 class="subheading">Cơ chế</h4>
            <xsl:choose>
              <xsl:when test="normalize-space(Interaction/Abbreviated)">
                <xsl:value-of select="Interaction/Abbreviated"/>
              </xsl:when>
              <xsl:when test="normalize-space(Interaction/Professional)">
                <xsl:value-of select="Interaction/Professional"/>
              </xsl:when>
            </xsl:choose>
            <xsl:apply-templates select="References">
              <xsl:with-param name="DivId" select="$DivId"/>
            </xsl:apply-templates>
          </xsl:element>
        </div>
      </xsl:element>
    </xsl:for-each>
  </xsl:template>
  <xsl:template match="Allergy">
    <xsl:param name="IntProd1"/>
    <div>
      <h4 class="subheading">Tiền sử dị ứng:</h4>
      <ol class="allergy-list">
        <xsl:for-each select="*">
          <xsl:choose>
            <xsl:when test="name()='CrossSensitive'">
              <xsl:variable name="DivId">
                c_<xsl:value-of select="translate($IntProd1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
              </xsl:variable>
              <li>
                The patient has a known history of allergic reaction to SubstanceClass, <em>
                  <xsl:value-of select="AllergySubstanceClass/@name|SubstanceClass/@name"/>
                </em>.<ul class="no-bullet-no-indent">
                  <li>
                    <em>
                      <xsl:value-of select="$IntProd1"/>
                    </em> contains <em>
                      <xsl:value-of select=".//PrescribingMolecule/@name"/>
                    </em>, which is cross-sensitive to<em>
                      <xsl:value-of select="AllergySubstanceClass/@name|SubstanceClass/@name"/>
                    </em>.
                  </li>
                  <xsl:apply-templates select=".//References">
                    <xsl:with-param name="DivId" select="$DivId"/>
                  </xsl:apply-templates>
                </ul>
              </li>
            </xsl:when>
            <xsl:when test="CrossSensitive">
              <xsl:variable name="DivId">
                c_<xsl:value-of select="translate($IntProd1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
              </xsl:variable>
              <li>
                The patient has a known history of allergic reaction to <xsl:value-of select="name()"/>, <em>
                  <xsl:value-of select="@name"/>
                </em>.<ul class="no-bullet-no-indent">
                  <li>
                    <em>
                      <xsl:value-of select="$IntProd1"/>
                    </em> contains <em>
                      <xsl:value-of select=".//PrescribingMolecule/@name"/>
                    </em>, which is cross-sensitive to <em>
                      <xsl:value-of select="@name"/>
                    </em>.
                  </li>
                  <xsl:apply-templates select=".//References">
                    <xsl:with-param name="DivId" select="$DivId"/>
                  </xsl:apply-templates>
                </ul>
              </li>
            </xsl:when>
            <xsl:when test="name()='ActiveCompositionGroup'">
              <!--							<ul> -->
              <xsl:for-each select="SubstanceClass">
                <!--							<li> -->
                <xsl:choose>
                  <xsl:when test="@name">
                    <li>
                      <em>
                        <xsl:value-of select="$IntProd1"/>
                      </em> contains <em>
                        <xsl:value-of select=".//PrescribingMolecule/@name"/>
                      </em>, which belongs to the same substance class as <em>
                        <xsl:value-of select="@name"/>
                      </em>.
                    </li>
                  </xsl:when>
                  <xsl:otherwise>
                    <li>
                      <em>
                        <xsl:value-of select="$IntProd1"/>
                      </em> contains <em>
                        <xsl:value-of select=".//PrescribingMolecule/@name"/>
                      </em>, which belongs to the same substance class as <em>
                        <xsl:value-of select="@name"/>
                      </em>.
                    </li>
                  </xsl:otherwise>
                </xsl:choose>
                <!--							</li>	-->
              </xsl:for-each>
              <!--							</ul> -->
            </xsl:when>
            <xsl:otherwise>
              <li>
                The patient has a known history of allergic reaction to <xsl:value-of select="name()"/>, <em>
                  <xsl:value-of select="@name"/>
                </em>.<ul class="no-bullet-no-indent">
                  <xsl:choose>
                    <xsl:when test="name()='Molecule' and .//PrescribingMolecule/@name!=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>, which belongs to the same substance class as <em>
                          <xsl:value-of select=".//SubstanceClass/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='Molecule' and .//PrescribingMolecule/@name=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='SubstanceClass'">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>, which belongs to that same substance class.<em>
                          <xsl:value-of select="@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='GenericItem' and .//PrescribingMolecule/@name!=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>, which belongs to the same substance class as <em>
                          <xsl:value-of select=".//SubstanceClass/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='GenericItem' and .//PrescribingMolecule/@name=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='GGPI' and .//PrescribingMolecule/@name!=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>, which belongs to the same substance class as <em>
                          <xsl:value-of select=".//SubstanceClass/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='GGPI' and .//PrescribingMolecule/@name=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='Product' and .//PrescribingMolecule/@name!=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>, which belongs to the same substance class as <em>
                          <xsl:value-of select=".//SubstanceClass/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                    <xsl:when test="name()='Product' and .//PrescribingMolecule/@name=@name">
                      <li>
                        <em>
                          <xsl:value-of select="$IntProd1"/>
                        </em> contains <em>
                          <xsl:value-of select=".//PrescribingMolecule/@name"/>
                        </em>.
                      </li>
                    </xsl:when>
                  </xsl:choose>
                </ul>
              </li>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </ol>
    </div>
  </xsl:template>
  <xsl:template match="DoseCheck|CombinedDoseCheck">
    <xsl:for-each select="*[count(Warnings) &gt; 0]">
      <xsl:variable name="IntProd" select="@name"/>
      <xsl:variable name="DivId">
        <xsl:value-of select="concat ('dc_', translate($IntProd, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_'), '_dc')"/>
      </xsl:variable>

      <xsl:element name="div">
        <xsl:attribute name="id">
          <xsl:value-of select="$DivId"/>
        </xsl:attribute>
        <h3>
          <a href="#">
            <xsl:if test="name(..)='CombinedDoseCheck'">
              <xsl:text>[Combined Dose Check] </xsl:text>
            </xsl:if>
            <xsl:value-of select="$IntProd"/>
          </a>
        </h3>
        <div>
          <xsl:for-each select="Warnings/Warning">
            <div class="ui-widget">
              <div class="ui-state-default ui-corner-all warning-bar">
                <p>
                  <span class="ui-icon ui-icon-alert"/>
                  <xsl:choose>
                    <xsl:when test="normalize-space(text())">
                      <xsl:value-of select="text()"/>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="Message"/>
                    </xsl:otherwise>
                  </xsl:choose>
                  <xsl:if test="ValidRoutes|ValidForms|ValidIndications">
                    <ul>
                      <xsl:for-each select="*/*">
                        <li>
                          <xsl:choose>
                            <xsl:when test="@name">
                              <xsl:value-of select="@name"/>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="text()"/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </li>
                      </xsl:for-each>
                    </ul>
                  </xsl:if>
                </p>
              </div>
            </div>
          </xsl:for-each>
        </div>
      </xsl:element>
    </xsl:for-each>
    <xsl:apply-templates select="CombinedDoseCheck"/>
  </xsl:template>
  <xsl:template match="Precaution">
    <li>
      <xsl:choose>
        <xsl:when test="normalize-space(./Abbreviated)">
          <xsl:value-of select="./Abbreviated"/>
        </xsl:when>
        <xsl:when test="normalize-space(./Professional)">
          <xsl:value-of select="./Professional"/>
        </xsl:when>
      </xsl:choose>
    </li>
  </xsl:template>
  <xsl:template match="DuplicateTherapy">
    <xsl:for-each select="./Warning[contains($DuplicateTherapyFilter, concat('^',@Level, '^'))]">
      <xsl:variable name="DuplicateLevel" select="translate(@Level, '123', '531')"/>
      <xsl:for-each select="./Duplicate">
        <!-- Convert the ATC code to uppercase -->
        <xsl:variable name="ATCCode" select="translate(@ATCCode,'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/>
        <xsl:variable name="DupPair">
          <xsl:for-each select="./*">
            <xsl:variable name="type" select="name()"/>
            <xsl:variable name="ref" select="@reference"/>
            <!-- Lookup the name from the reference -->
            <em>
              <xsl:value-of select="/Result/Interaction/*[@reference=$ref]/@name"/>
            </em>
            <xsl:if test="position()=1">
              <xsl:text> and </xsl:text>
            </xsl:if>
          </xsl:for-each>
        </xsl:variable>
        <xsl:element name="div">
          <xsl:attribute name="id">
            d_<xsl:value-of select="$DuplicateLevel"/>_<xsl:value-of select="translate($DupPair, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>_<xsl:value-of select="translate($DupPair, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
          </xsl:attribute>
          <h3>
            <a>
              <xsl:call-template name="SeverityColor">
                <xsl:with-param name="SeverityLevel" select="$DuplicateLevel"/>
              </xsl:call-template>
              <xsl:value-of select="$DupPair"/> [ATC Code: <xsl:value-of select="$ATCCode"/>]
            </a>
          </h3>
        </xsl:element>
      </xsl:for-each>
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="DuplicateIngredient">
    <!--Applying the when condition to validate the xml response to execute DI old style sheet tempalte or new style sheet-->
    <xsl:choose>
      <xsl:when test="count(//Interaction/DuplicateIngredient/Warning/Molecule/Molecule/*) &lt;= 0">
        <!-- Add XSLT instructions for the first case -->
        <xsl:for-each select="./Warning[contains($DuplicateIngredientFilter, concat('^',@Level, '^'))]">
          <xsl:variable name="DuplicateLevel" select="translate(@Level, '123', '531')"/>
          <xsl:for-each select="./Molecule">
            <xsl:variable name="DupPair">
              <xsl:for-each select="./*">
                <xsl:variable name="type" select="name()"/>
                <xsl:variable name="ref" select="@reference"/>
                <!-- Lookup the name from the reference -->
                <em>
                  <xsl:value-of select="/Result/Interaction/*[@reference=$ref]/@name"/>
                </em>
                <xsl:if test="position()=1">
                  <xsl:text> and </xsl:text>
                </xsl:if>
              </xsl:for-each>
            </xsl:variable>
            <xsl:element name="div">
              <xsl:attribute name="id">
                d_
                <xsl:value-of select="$DuplicateLevel"/>_
                <xsl:value-of select="translate($DupPair, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>_
                <xsl:value-of select="translate($DupPair, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
              </xsl:attribute>
              <h3>
                <a>
                  <xsl:call-template name="SeverityColor">
                    <xsl:with-param name="SeverityLevel" select="$DuplicateLevel"/>
                  </xsl:call-template>
                  <xsl:value-of select="$DupPair"/> [
                  <xsl:value-of select="@name"/>]

                </a>
              </h3>
            </xsl:element>
          </xsl:for-each>
        </xsl:for-each>
      </xsl:when>
      <xsl:when test="count(//Interaction/DuplicateIngredient/Warning/Molecule/Molecule/*) &gt;= 1">
        <!-- Add XSLT instructions for the second case -->
        <xsl:for-each select="./Warning[contains($DuplicateIngredientFilter, concat('^',@Level, '^'))]">
          <xsl:variable name="DuplicateLevel" select="translate(@Level, '123', '531')"/>
          <!--step to pick root molecule names and store in a xsl Dupair1 variable-->
          <xsl:variable name="DupPair1">
            <xsl:for-each select="./Molecule">
              <xsl:value-of select="@name"/>
            </xsl:for-each>
          </xsl:variable>


          <xsl:for-each select="./Molecule/Molecule">
            <xsl:variable name="DupPair">
              <xsl:for-each select="./*">
                <xsl:variable name="type" select="name()"/>
                <xsl:variable name="ref" select="@reference"/>
                <!-- Lookup the name from the reference -->
                <em>
                  <xsl:value-of select="/Result/Interaction/*[@reference=$ref]/@name"/>
                </em>

                <xsl:if test="position()=1">
                  <!--here displaying the Duppair1 root moleculnames-->
                  [<xsl:value-of select="$DupPair1"/>]<xsl:text> vs </xsl:text>
                </xsl:if>
              </xsl:for-each>
            </xsl:variable>
            <xsl:element name="div">
              <xsl:attribute name="id">
                d_<xsl:value-of select="$DuplicateLevel"/>_<xsl:value-of select="translate($DupPair, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>_<xsl:value-of select="translate($DupPair1, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ ','abcdefghijklmnopqrstuvwxyz_')"/>
              </xsl:attribute>

              <h3>
                <a>
                  <xsl:call-template name="SeverityColor">
                    <xsl:with-param name="SeverityLevel" select="$DuplicateLevel"/>
                  </xsl:call-template>
                  <xsl:value-of select="$DupPair"/> [<xsl:value-of select="@name"/>]
                </a>
              </h3>
            </xsl:element>
          </xsl:for-each>

        </xsl:for-each>
      </xsl:when>
    </xsl:choose>

  </xsl:template>
  <xsl:template match="References">
    <xsl:param name="DivId"/>
    <h4 class="subheading">Tài liệu tham khảo</h4>
    <ul>
      <xsl:apply-templates select="JournalReference"/>
      <xsl:apply-templates select="ElectronicReference"/>
      <xsl:apply-templates select="BookReference"/>
      <xsl:apply-templates select="WebReference"/>
    </ul>
  </xsl:template>
  <!-- Need disable-output-escaping="yes" to preserve foreign names -->
  <xsl:template match="JournalReference">
    <li>
      <xsl:value-of select="Author" disable-output-escaping="yes"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="Title"/>
      <xsl:text>. </xsl:text>
      <i>
        <xsl:value-of select="Journal"/>
      </i>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="Year"/>
      <xsl:text>; </xsl:text>
      <xsl:value-of select="Volume"/>
      <xsl:if test="string-length(Part) &gt; 0">
        <xsl:text>(</xsl:text>
        <xsl:value-of select="Part"/>
        <xsl:text>)</xsl:text>
      </xsl:if>
      <xsl:text>:</xsl:text>
      <xsl:value-of select="Page"/>
    </li>
  </xsl:template>
  <xsl:template match="ElectronicReference">
    <li>
      <xsl:value-of select="Title"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="RefTitle"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="SystemName"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="SystemAuthor" disable-output-escaping="yes"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="SystemManf"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="SystemManfLoc"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="SystemManfCountry"/>
      <xsl:text>. </xsl:text>
      <xsl:text> Available from URL: </xsl:text>
      <a>
        <xsl:attribute name="href">
          <xsl:value-of select="URL"/>
        </xsl:attribute>
        <xsl:value-of select="URL"/>
      </a>
      <xsl:text>. (Accessed on </xsl:text>
      <xsl:value-of select="Date"/>
      <xsl:text>)</xsl:text>
    </li>
  </xsl:template>
  <xsl:template match="BookReference">
    <li>
      <xsl:value-of select="Author" disable-output-escaping="yes"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="ChapterTitle"/>
      <xsl:text>. In: </xsl:text>
      <xsl:value-of select="Title"/>
      <xsl:text>. </xsl:text>
      <xsl:value-of select="Edition"/>
      <xsl:text> ed. </xsl:text>
      <xsl:if test="string-length(Volume) &gt; 0">
        <xsl:value-of select="Volume"/>
        <xsl:text>. </xsl:text>
      </xsl:if>
      <xsl:value-of select="PublishLoc"/>
      <xsl:text>: </xsl:text>
      <xsl:value-of select="Publisher"/>
      <xsl:text>; </xsl:text>
      <xsl:value-of select="Year"/>
      <xsl:text>; </xsl:text>
      <xsl:value-of select="Page"/>
      <xsl:text>. </xsl:text>
    </li>
  </xsl:template>
  <xsl:template match="WebReference">
    <li>
      <xsl:value-of select="Title"/>
      <xsl:text>. </xsl:text>
      <i>
        <xsl:value-of select="Site"/>
        <xsl:text>. </xsl:text>
      </i>
      <xsl:text> URL: </xsl:text>
      <a>
        <xsl:attribute name="href">
          <xsl:value-of select="URL"/>
        </xsl:attribute>
        <xsl:value-of select="URL"/>
      </a>
      <xsl:text>. (Accessed on </xsl:text>
      <xsl:value-of select="Date"/>
      <xsl:text>)</xsl:text>
    </li>
  </xsl:template>
  <xsl:template name="SeverityColor">
    <xsl:param name="SeverityLevel"/>
    <table class="severity">
      <tr>
        <xsl:element name="td">
          <xsl:attribute name="class">
            <xsl:value-of select="concat('severity', $SeverityLevel)"/>
          </xsl:attribute>
        </xsl:element>
      </tr>
    </table>&#160;
  </xsl:template>
  <xsl:template name="d2dSeverity">
    <xsl:param name="Severity"/>
    <h4 class="subheading">Mức độ</h4>
    <p>
      <em>
        <xsl:value-of select="Severity/@name"/>
      </em>
      <xsl:text> - </xsl:text>
      <xsl:apply-templates select="$d2d_sev_level-top">
        <xsl:with-param name="SeverityLevel" select="$Severity"/>
      </xsl:apply-templates>
    </p>
  </xsl:template>
  <xsl:template match="mims:d2d_sev_levels">
    <xsl:param name="SeverityLevel"/>
    <xsl:value-of select="key('d2d_sev_level-lookup', $SeverityLevel)/text()"/>
  </xsl:template>
  <xsl:template name="d2hSeverity">
    <xsl:param name="Severity"/>
    <h4 class="subheading">Mức độ</h4>
    <p>
      <em>
        <xsl:value-of select="Severity/@name"/>
      </em>
      <xsl:text> - </xsl:text>
      <xsl:apply-templates select="$d2h_sev_level-top">
        <xsl:with-param name="SeverityLevel" select="$Severity"/>
      </xsl:apply-templates>
    </p>
  </xsl:template>
  <xsl:template match="mims:d2l_sev_levels">
    <xsl:param name="SeverityLevel"/>
    <xsl:value-of select="key('d2l_sev_level-lookup', $SeverityLevel)/text()"/>
  </xsl:template>
  <xsl:template name="d2lSeverity">
    <xsl:param name="Severity"/>
    <h4 class="subheading">Mức độ</h4>
    <em>
      <xsl:value-of select="Severity/@name"/>
    </em>
    <xsl:text> - </xsl:text>
    <xsl:apply-templates select="$d2l_sev_level-top">
      <xsl:with-param name="SeverityLevel" select="$Severity"/>
    </xsl:apply-templates>
  </xsl:template>
  <xsl:template match="mims:d2h_sev_levels">
    <xsl:param name="SeverityLevel"/>
    <xsl:value-of select="key('d2h_sev_level-lookup', $SeverityLevel)/text()"/>
  </xsl:template>
  <xsl:template name="docLevel">
    <xsl:param name="DocLevel"/>
    <h4 class="subheading">Mức độ bằng chứng</h4>
    <p>
      <em>
        <xsl:value-of select="Documentation/@name"/>
      </em>
      <xsl:text> - </xsl:text>
      <xsl:apply-templates select="$doc_level-top">
        <xsl:with-param name="DocLevel" select="$DocLevel"/>
      </xsl:apply-templates>
    </p>
  </xsl:template>
  <xsl:template match="mims:doc_levels">
    <xsl:param name="DocLevel"/>
    <xsl:value-of select="key('doc_level-lookup', $DocLevel)/text()"/>
  </xsl:template>
  <xsl:template match="mims:preg_cats">
    <xsl:param name="Source"/>
    <xsl:param name="Cat"/>
    <xsl:value-of select="key('preg_cat-lookup', concat($Source, '!', $Cat))/text()"/>
  </xsl:template>
  <!-- 
	<xsl:template match="/Result/Detail/Product">
		<STYLE>BODY {COLOR: #6B696B; FONT-FAMILY: Tahoma;FONT-SIZE: 9pt;}  TR.clsOdd { background-Color: beige; }  TR.clsEven { background-color: #cccccc; } TD.FirstColumn{ background-Color: beige;FONT-FAMILY: Tahoma; FONT-SIZE: 9pt;COLOR:black; }  TR.Header{ background-Color: beige;FONT-FAMILY: Tahoma; FONT-SIZE: 9pt;COLOR:black; }  table{FONT-FAMILY:Tahoma; FONT-SIZE:10pt;COLOR: #6B696B;}</STYLE>
		<p align="center">
			<strong>
				<xsl:value-of select="@name"/>
			</strong>
			<br/>
			<xsl:if test="string-length(GGPI/@name) &gt;  0">
				<xsl:text>(</xsl:text>
				<xsl:value-of select="GGPI/@name"/>
				<xsl:text>)</xsl:text>
			</xsl:if>
			<br/>
		</p>
		<table width="100%">
      <tr>
				<td/>
				<td colspan="5">				</td>
			</tr>
			<tr>
				<td>
					<xsl:text> </xsl:text>
				</td>
			</tr>
			<xsl:if test="string-length(@code) &gt;  0">
				<tr>
					<td class="FirstColumn">Mã</td>
					<td>
						<xsl:value-of select="@code"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="string-length(@registrationDate) &gt;  0">
				<tr>
					<td class="FirstColumn">Ngày đăng ký</td>
					<td>
						<xsl:value-of select="@registrationDate"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="string-length(@onMarketDate) &gt;  0">
				<tr>
					<td class="FirstColumn">Ngày đưa ra thị trường</td>
					<td>
						<xsl:value-of select="@onMarketDate"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="string-length(@offMarketDate) &gt;  0">
				<tr>
					<td class="FirstColumn">Ngày rút khỏi thị trường</td>
					<td>
						<xsl:value-of select="@offMarketDate"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="string-length(@hardStop) &gt;  0">
				<tr>
					<td class="FirstColumn">Thu hồi</td>
					<td>
						<xsl:value-of select="@hardStop"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:for-each select="Items/Item">
				<xsl:for-each select="Routes/Route">
					<xsl:if test="string-length(@name) &gt;  0">
						<tr>
							<td class="FirstColumn">Đường dùng</td>
							<td>
								<xsl:value-of select="@name"/>
							</td>
						</tr>
					</xsl:if>
				</xsl:for-each>
				<xsl:if test="string-length(Form/@name) &gt;  0">
					<tr>
						<td class="FirstColumn">Dạng bào chế</td>
						<td>
							<xsl:value-of select="Form/@name"/>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="string-length(ActiveCompositionGroup/GenericItem/HL7Form/@name) &gt;  0">
					<tr>
						<td class="FirstColumn">Dạng bào chế theo HL7</td>
						<td>
							<xsl:value-of select="ActiveCompositionGroup/GenericItem/HL7Form/@name"/>
						</td>
					</tr>
				</xsl:if>
				<tr>
					<td class="FirstColumn">Phân tử</td>
					<td>
						<table>
							<tr class="Header">
								<td>Tên</td>
								<td>Hàm lượng</td>
								<td>Đơn vị</td>
								<xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolume) &gt;  0">
									<td>Mỗi đơn vị thể tích</td>
								</xsl:if>
								<xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolumeUnit) &gt;  0">
									<td>Đơn vị</td>
								</xsl:if>
							</tr>
							<xsl:for-each select="ActiveCompositionGroup/ActiveComposition/Molecules/Molecule">
								<tr>
									<td>
										<xsl:value-of select="@name"/>
									</td>
									<td>
										<xsl:value-of select="@strength"/>
									</td>
									<td>
										<xsl:value-of select="@strengthUnit"/>
									</td>
									<xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolume) &gt;  0">
										<td>
											<xsl:value-of select="@perVolume"/>
										</td>
									</xsl:if>
									<xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolumeUnit) &gt;  0">
										<td>
											<xsl:value-of select="@perVolumeUnit"/>
										</td>
									</xsl:if>
								</tr>
							</xsl:for-each>
						</table>
					</td>
				</tr>
				<xsl:if test="string-length(ActiveCompositionGroup/GenericItem/@name) &gt;  0">
					<tr>
						<td class="FirstColumn">Hoạt chất</td>
						<td>
							<xsl:value-of select="ActiveCompositionGroup/GenericItem/@name"/>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="Images/@count &gt;  0">
					<tr>
						<td class="FirstColumn">Hình ảnh</td>
						<td>
							<img alt="PIC" src="C:\FTImage.gif"/>
						</td>
					</tr>
				</xsl:if>
			</xsl:for-each>
			<tr>
				<td class="FirstColumn">Nhóm trị liệu</td>
				<td>
					<ui>
						<xsl:for-each select="TherapeuticClasses/TherapeuticClass">
							<li>
								<xsl:value-of select="@name"/>
							</li>
						</xsl:for-each>
					</ui>
				</td>
			</tr>
			<xsl:if test="ATCCodes/@count &gt;  0">
				<tr>
					<td class="FirstColumn">Mã ATC</td>
					<td>
						<table width="100%">
							<tr class="Header">
								<td class="FirstColumn" width="70%">name</td>
								<td class="FirstColumn">code</td>
							</tr>
							<xsl:for-each select="ATCCodes/ATCCode">
								<tr>
									<td>
										<xsl:value-of select="@name"/>
									</td>
									<td>
										<xsl:value-of select="@code"/>
									</td>
								</tr>
							</xsl:for-each>
							<xsl:for-each select="ATCCodes/AtcClassification">
								<tr>
									<td>
										<xsl:value-of select="@name"/>
									</td>
									<td>
										<xsl:value-of select="@code"/>
									</td>
								</tr>
							</xsl:for-each>
						</table>
					</td>
				</tr>
			</xsl:if>
			<tr>
				<td class="FirstColumn">Công ty</td>
				<td>
					<table width="100%">
						<tr class="Header">
							<td class="FirstColumn" width="70%">Tên</td>
							<td class="FirstColumn">Loại</td>
						</tr>
						<xsl:for-each select="Companies/Company">
							<tr>
								<td>
									<xsl:value-of select="@name"/>
								</td>
								<td>
									<xsl:value-of select="CompanyType/@name"/>
								</td>
							</tr>
						</xsl:for-each>
					</table>
				</td>
			</tr>
		</table>
	</xsl:template>-->
  <xsl:template match="mims:preg_pics">
    <xsl:param name="Source"/>
    <xsl:param name="Cat"/>
    <xsl:value-of select="key('preg_pic-lookup', concat($Source, '!', $Cat))/text()"/>
  </xsl:template>
  <!-- Lookup tables -->
  <mims:d2d_sev_levels>
    <mims:d2d_sev_level name="2">Tương tác có thể xảy ra dựa theo cơ chế hoạt động của các thuốc được sử dụng đồng thời. Cần chú ý việc tăng hoặc giảm tác dụng, liên quan đến việc kết hợp các thuốc này.</mims:d2d_sev_level>
    <mims:d2d_sev_level name="3">Tác dụng lâm sàng của tương tác ít gặp và có thể gây khó chịu nhưng thường không cần thay đổi nhiều trong điều trị. Bệnh nhân nên được theo dõi các biểu hiện của tương tác có thể xảy ra.</mims:d2d_sev_level>
    <mims:d2d_sev_level name="4">Tương tác giữa các thuốc này có thể làm tình trạng của bệnh nhân chuyển biến xấu hơn. Bệnh nhân nên được theo dõi các biểu hiện của tương tác có thể xảy ra. Có thể cần can thiệp y khoa hoặc thay đổi phác đồ điều trị.</mims:d2d_sev_level>
    <mims:d2d_sev_level name="5">Tương tác giữa các thuốc này có thể đe dọa tính mạng hoặc gây tổn thương vĩnh viễn. Những thuốc này thường không được sử dụng chung; có thể cần can thiệp y khoa.</mims:d2d_sev_level>
  </mims:d2d_sev_levels>
  <mims:d2h_sev_levels>
    <mims:d2h_sev_level name="3">Chống chỉ định tuyệt đối; việc sử dụng thuốc hoàn toàn không được khuyến cáo và nên tránh dùng.</mims:d2h_sev_level>
    <mims:d2h_sev_level name="2">Không khuyến cáo dùng thuốc và bệnh nhân có nguy cơ cao gặp tác dụng không mong muốn hoặc tình trạng bệnh hiện tại có thể diễn tiến xấu hơn, tuy nhiên, vẫn có thể dùng thuốc và nguy cơ có thể thấp hơn so với lợi ích khi cân nhắc các yếu tố khác. </mims:d2h_sev_level>
  </mims:d2h_sev_levels>
  <mims:d2l_sev_levels>
    <mims:d2l_sev_level name="1">Có thể đã có hoặc chưa có thử nghiệm trên người nhưng các thuốc này có thể gây độc tính nặng cho trẻ bú sữa mẹ. Không nên dùng cho phụ nữ cho con bú. Các thuốc này cũng bao gồm thuốc có chống chỉ định dùng khi cho con bú.</mims:d2l_sev_level>
    <mims:d2l_sev_level name="2">Các thuốc này được báo cáo là gây tác dụng không mong muốn có ý nghĩa trên lâm sàng đối với trẻ bú sữa mẹ, và/hoặc dữ liệu hiện có trên động vật gợi ý có nguy cơ đáng kể đối với trẻ bú sữa mẹ. Các thuốc này cũng có thể ức chế tiết sữa hoặc có thể gây hại cho người mẹ. Nếu không thể tránh việc sử dụng trong thời kỳ cho con bú, cần theo dõi độc tính có thể xảy ra đối với cả người mẹ hoặc đứa trẻ.</mims:d2l_sev_level>
    <mims:d2l_sev_level name="3">Các thuốc này trên lý thuyết có thể gây tác dụng không mong muốn cho trẻ bú sữa mẹ nhưng tác dụng này chưa được quan sát thấy hoặc chỉ thỉnh thoảng có tác dụng không mong muốn nhẹ. Có thể dùng thuốc này cho phụ nữ cho con bú nhưng cần theo dõi độc tính có thể xảy ra đối với cả người mẹ hoặc đứa trẻ.</mims:d2l_sev_level>
  </mims:d2l_sev_levels>
  <mims:doc_levels>
    <mims:doc_level name="4">Có một số báo cáo về tương tác này. Giải thích dựa trên dược lý của nguyên nhân xảy ra tương tác đã được ghi nhận và hiểu rõ. Các nghiên cứu có kiểm soát thông thường đã xác nhận rằng tương tác này có xảy ra.</mims:doc_level>
    <mims:doc_level name="3">Mặc dù có thể chưa thực hiện các nghiên cứu được kiểm soát, một vài báo cáo riêng lẻ đã được ghi nhận và các dữ liệu khác gợi ý rõ ràng rằng có xảy ra tương tác này.</mims:doc_level>
    <mims:doc_level name="2">Có ít báo cáo về tương tác này. Các báo cáo này thường là báo cáo riêng lẻ với số lượng ít, trong đó có ghi nhận ảnh hưởng trên lâm sàng của tương tác.</mims:doc_level>
    <mims:doc_level name="1">Tương tác có thể đã xảy ra với các thuốc khác thuộc cùng nhóm dược lý, hoặc theo lý thuyết có khả năng xảy ra tương tác.</mims:doc_level>
  </mims:doc_levels>
  <mims:preg_cats>
    <mims:preg_cat cat="FDA!A">Các nghiên cứu được kiểm soát tốt và đầy đủ trên người đã không chứng minh được là có nguy cơ cho bào thai trong 3 tháng đầu thai kỳ (và không có bằng chứng về nguy cơ trong các tháng sau đó).</mims:preg_cat>
    <mims:preg_cat cat="FDA!B">Các nghiên cứu về sinh sản ở động vật đã không chứng minh được là có nguy cơ cho bào thai và không có nghiên cứu được kiểm soát tốt và đầy đủ ở phụ nữ mang thai HOẶC Các nghiên cứu trên động vật đã cho thấy có tác dụng không mong muốn, nhưng các nghiên cứu được kiểm soát tốt và đầy đủ ở phụ nữ mang thai không chứng minh được là có nguy cơ cho bào thai trong bất kỳ giai đoạn nào của thai kỳ.</mims:preg_cat>
    <mims:preg_cat cat="FDA!C">Các nghiên cứu về sinh sản ở động vật đã cho thấy có tác dụng không mong muốn cho bào thai và không có nghiên cứu được kiểm soát tốt và đầy đủ trên người, nhưng vì lợi ích có thể mang lại nên có thể dùng thuốc cho phụ nữ mang thai mặc dù có nguy cơ tiềm tàng.</mims:preg_cat>
    <mims:preg_cat cat="FDA!D">Có bằng chứng về nguy cơ cho bào thai người dựa trên dữ liệu về phản ứng không mong muốn từ ghi nhận trong nghiên cứu hoặc khi lưu hành trên thị trường hoặc từ nghiên cứu trên người, nhưng vì lợi ích có thể mang lại nên có thể dùng thuốc cho phụ nữ mang thai mặc dù có nguy cơ tiềm tàng.</mims:preg_cat>
    <mims:preg_cat cat="FDA!X">Các nghiên cứu trên động vật hoặc người đã chứng minh gây bất thường cho bào thai và/hoặc có bằng chứng về nguy cơ trên bào thai người dựa trên dữ liệu về phản ứng không mong muốn từ ghi nhận trong nghiên cứu hoặc khi lưu hành trên thị trường, và nguy cơ liên quan đến việc dùng thuốc cho phụ nữ mang thai rõ ràng cao hơn so với lợi ích tiềm tàng.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!A">Drugs which have been taken by a large number of pregnant women and women of childbearing age without an increase in the frequency of malformations or other direct or indirect harmful effects on the fetus having been observed.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!B1">Drugs which have been taken by only a limited number of pregnant women and women of childbearing age, without an increase in the frequency of malformation or other direct or indirect harmful effects on the human fetus having been observed. Studies in animals have not shown evidence of an increased occurrence of fetal damage.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!B2">Drugs which have been taken by only a limited number of pregnant women and women of childbearing age, without an increase in the frequency of malformation or other direct or indirect harmful effects on the human fetus having been observed. Studies in animals are inadequate or may be lacking, but available data show no evidence of an increased occurrence of fetal damage.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!B3">Drugs which have been taken by only a limited number of pregnant women and women of childbearing age, without an increase in the frequency of malformation or other direct or indirect harmful effects on the human fetus having been observed. Studies in animals have shown evidence of an increased occurrence of fetal damage, the significance of which is considered uncertain in humans.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!C">Drugs which, owing to their pharmaceutical effects, have caused or may be suspected of causing, harmful effects on the human fetus or neonate without causing malformations. These effects may be reversible.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!D">Drugs which have caused, are suspected to have caused or may be expected to cause, an increased incidence of human fetal malformations or irreversible damage. These drugs may also have adverse pharmacological effects.</mims:preg_cat>
    <mims:preg_cat cat="ADEC!X">Drugs that have such a high risk of causing permanent damage to the fetus that they should NOT be used in pregnancy or when there is a possibility of pregnancy.</mims:preg_cat>
  </mims:preg_cats>
  <mims:preg_pics>
    <mims:preg_pic cat="FDA!A">images/cata.gif</mims:preg_pic>
    <mims:preg_pic cat="FDA!B">images/catb.gif</mims:preg_pic>
    <mims:preg_pic cat="FDA!C">images/catc.gif</mims:preg_pic>
    <mims:preg_pic cat="FDA!D">images/catd.gif</mims:preg_pic>
    <mims:preg_pic cat="FDA!X">images/catx.gif</mims:preg_pic>
  </mims:preg_pics>
  <xsl:template match="Content|Detail">

    <xsl:choose>
      <xsl:when test="count(*)=0">
        <div class="ui-widget">
          <div class="ui-state-default ui-corner-all warning-bar">
            <p>
              <span class="ui-icon ui-icon-alert"/>Không có thông tin kê đơn.
            </p>
          </div>
        </div>
      </xsl:when>
      <xsl:otherwise>
        <div id="tabs">
          <ul>
		    		  <div class="container1">
  <img src="images/MIMSIntegratedfinallogo.png" alt="Logo" style="width:150px; height:50px; cursor:pointer;" />
  <span>
    <xsl:value-of select="//Result/@copyright"/>
    <xsl:text> | Database version/BuildID: </xsl:text>
    <xsl:value-of select="//Result/@buildID"/>
    <xsl:text> | Expiry date: </xsl:text>
    <xsl:value-of select="//Result/@expiryDate"/>
  </span>
</div>
            <xsl:if test="//BRIEFMONO">
              <li>
                <a href="#tab-brief">Thông tin tóm tắt</a>
              </li>
            </xsl:if>
            <xsl:if test="//FULLMONO">
              <li>
                <a href="#tab-full">Thông tin tiếng Anh</a>
              </li>
            </xsl:if>
            <xsl:if test="//MONOGRAPH">
              <li>
                <a href="#tab-generic">Hoạt chất</a>
              </li>
            </xsl:if>
            <xsl:if test="//Detail/Product">
              <li>
                <a href="#tab-detail">Product Details</a>
              </li>
            </xsl:if>
            <xsl:if test="//Detail/Package">
              <li>
                <a href="#tab-package">Package Details</a>
              </li>
            </xsl:if>
            <xsl:if test="//ImageData">
              <li>
                <a href="#tab-drugimages">Drug Image</a>
              </li>
            </xsl:if>
            <xsl:if test="//VIDAL">
              <li>
                <a href="#tab-vidal">Thông tin chi tiết</a>
              </li>
            </xsl:if>
            <xsl:if test="//PILS/SPECIFICPIL">
              <li>
                <a href="#tab-pils">PILS</a>
              </li>
            </xsl:if>
          </ul>
          <xsl:if test="//FULLMONO">
            <div id="tab-full">
              <xsl:apply-templates select="//FULLMONO"/>
            </div>
          </xsl:if>
          <xsl:if test="//BRIEFMONO">
            <div id="tab-brief">
              <xsl:apply-templates select="//BRIEFMONO"/>
            </div>
          </xsl:if>
          <xsl:if test="//MONOGRAPH">
            <div id="tab-generic">
              <xsl:apply-templates select="//MONOGRAPH"/>
            </div>
          </xsl:if>
          <xsl:if test="//Detail/Product">
            <div id="tab-detail">
              <xsl:apply-templates select="//Detail/Product"/>
            </div>
          </xsl:if>
          <xsl:if test="//Detail/Package">
            <div id="tab-package">
              <xsl:apply-templates select="//Detail/Package/PackageType"/>
            </div>
          </xsl:if>
          <xsl:if test="//VIDAL">
            <div id="tab-vidal">
              <xsl:apply-templates select="//VIDAL"/>
            </div>
          </xsl:if>
          <xsl:if test="//PILS/SPECIFICPIL">
            <div id="tab-pils">
              <xsl:apply-templates select="//PILS/SPECIFICPIL"/>
            </div>
          </xsl:if>
          <xsl:if test="/Result/Content/ImageData">
            <div id="tab-drugimages">
              <xsl:apply-templates select="/Result/Content/ImageData"/>
            </div>
          </xsl:if>
        </div>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <!-- For Monograph -->
  <xsl:template match="BRIEFMONO">
    <h4 class="subheading">
      <xsl:value-of select="BRDNAME"/>
    </h4>
    <table class="ui-widget monograph">
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thành phần</xsl:with-param>
        <xsl:with-param name="SectionData" select="BC"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="BI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Liều lượng và cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="BD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chống chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="BCI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thận trọng</xsl:with-param>
        <xsl:with-param name="SectionData" select="BSP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phản ứng có hại của thuốc</xsl:with-param>
        <xsl:with-param name="SectionData" select="BAR"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tương tác</xsl:with-param>
        <xsl:with-param name="SectionData" select="BDI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Danh mục chất độc</xsl:with-param>
        <xsl:with-param name="SectionData" select="BPOI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="Form/Advice"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chi tiết cách sử dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="Form/Remark"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Đóng gói/ Trình bày</xsl:with-param>
        <xsl:with-param name="SectionData" select="BPP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phân loại nguy cơ trong thai kỳ</xsl:with-param>
        <xsl:with-param name="SectionData" select="BPCAT"/>
      </xsl:call-template>
    </table>
  </xsl:template>
  <xsl:template match="FULLMONO">
    <h4 class="subheading">
      <xsl:value-of select="BRDNAME"/>
    </h4>
    <table class="ui-widget monograph">
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thành phần</xsl:with-param>
        <xsl:with-param name="SectionData" select="FC"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Mô tả</xsl:with-param>
        <xsl:with-param name="SectionData" select="FDESC"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="FI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Liều lượng và cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="FD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Quá liều</xsl:with-param>
        <xsl:with-param name="SectionData" select="FOD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chống chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="FCI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thận trọng</xsl:with-param>
        <xsl:with-param name="SectionData" select="FSP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phản ứng có hại của thuốc</xsl:with-param>
        <xsl:with-param name="SectionData" select="FAR"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tác dụng không mong muốn</xsl:with-param>
        <xsl:with-param name="SectionData" select="FSE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tương tác</xsl:with-param>
        <xsl:with-param name="SectionData" select="FDI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cơ chế tác dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="FA"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thận trọng khi sử dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="FCAU"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Bảo quản</xsl:with-param>
        <xsl:with-param name="SectionData" select="FSTO"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Danh mục chất độc</xsl:with-param>
        <xsl:with-param name="SectionData" select="FPOI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Đóng gói/ Trình bày</xsl:with-param>
        <xsl:with-param name="SectionData" select="FPP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cảnh báo</xsl:with-param>
        <xsl:with-param name="SectionData" select="FW"/>
      </xsl:call-template>
    </table>
  </xsl:template>
  <xsl:template match="MONOGRAPH">
    <h4 class="subheading">
      <xsl:value-of select="../@name"/>
    </h4>
    <table class="ui-widget monograph">
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thành phần</xsl:with-param>
        <xsl:with-param name="SectionData" select="GENMONO"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="BI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Liều lượng và cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="GDOSE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="GPPPA"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chống chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="GCI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thận trọng</xsl:with-param>
        <xsl:with-param name="SectionData" select="GSP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phản ứng có hại của thuốc</xsl:with-param>
        <xsl:with-param name="SectionData" select="GAR"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tương tác</xsl:with-param>
        <xsl:with-param name="SectionData" select="GDI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cơ chế tác dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="GACTION"/>
      </xsl:call-template>

      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Special Patient Group</xsl:with-param>
        <xsl:with-param name="SectionData" select="GSPG"/>
      </xsl:call-template>

      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Danh mục chất độc</xsl:with-param>
        <xsl:with-param name="SectionData" select="BPOI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thay đổi chỉ số xét nghiệm</xsl:with-param>
        <xsl:with-param name="SectionData" select="GLAB"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tương tác với thức ăn</xsl:with-param>
        <xsl:with-param name="SectionData" select="GFOOD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phân loại MIMS</xsl:with-param>
        <xsl:with-param name="SectionData" select="translate(GCLS, '*', '')"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phân loại nguy cơ trong thai kỳ</xsl:with-param>
        <xsl:with-param name="SectionData" select="GPCAT"/>
      </xsl:call-template>

      <xsl:if test="normalize-space(//Content/*/MONOGRAPH/GATC1) != ''">
        <xsl:call-template name="ShowSection">
          <xsl:with-param name="SectionName">ATC Classification and Code</xsl:with-param>
          <xsl:with-param name="SectionData" select="concat(//Content/*/MONOGRAPH/GATC1, '- ', '(',  //Content/*/MONOGRAPH/GATC2, ')')"/>
        </xsl:call-template>
      </xsl:if>
      <!--normalize-space(//Content/*/MONOGRAPH/GATC1)!=‘’
			<xsl:call-template name="ShowSection">
			
				<xsl:with-param name="SectionName">ATC Classification and Code</xsl:with-param>
				<xsl:with-param name="SectionData" select= "concat(//Content/*/MONOGRAPH/GATC1, '- ', '(',  //Content/*/MONOGRAPH/GATC2, ')')"/>
				
			</xsl:call-template>-->

    </table>
  </xsl:template>
  <xsl:template match="//Detail/Product">
    <!--<xsl:if test="position() = 1">-->
    <!-- Add this condition to ensure uniqueness -->
    <h4 class="subheading">
      <xsl:value-of select="//Detail/Product/@name"/>
    </h4>

    <table class="ui-widget monograph">
      <xsl:if test="string-length(@code) &gt;  0">
        <tr>
          <td class="FirstColumn">Code</td>
          <td>
            <xsl:value-of select="@code"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="string-length(@registrationDate) &gt;  0">
        <tr>
          <td class="FirstColumn">Registration Date</td>
          <td>
            <xsl:value-of select="@registrationDate"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="string-length(@onMarketDate) &gt;  0">
        <tr>
          <td class="FirstColumn">On Market Date</td>
          <td>
            <xsl:value-of select="@onMarketDate"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="string-length(@offMarketDate) &gt;  0">
        <tr>
          <td class="FirstColumn">Off Market Date</td>
          <td>
            <xsl:value-of select="@offMarketDate"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="string-length(@hardStop) &gt;  0">
        <tr>
          <td class="FirstColumn">Hard Stop</td>
          <td>
            <xsl:value-of select="@hardStop"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:for-each select="Items/Item">
        <xsl:for-each select="Routes/Route">
          <xsl:if test="string-length(@name) &gt;  0">
            <tr>
              <xsl:call-template name="ShowSection">
                <xsl:with-param name="SectionName">Route</xsl:with-param>
                <xsl:with-param name="SectionData" select="@name"/>
              </xsl:call-template>

            </tr>
          </xsl:if>
        </xsl:for-each>
        <xsl:if test="string-length(Form/@name) &gt;  0">
          <tr>
            <xsl:call-template name="ShowSection">
              <xsl:with-param name="SectionName">Form</xsl:with-param>
              <xsl:with-param name="SectionData" select="Form/@name"/>
            </xsl:call-template>

          </tr>
        </xsl:if>
        <xsl:if test="string-length(ActiveCompositionGroup/GenericItem/HL7Form/@name) &gt;  0">
          <tr>
            <xsl:call-template name="ShowSection">
              <xsl:with-param name="SectionName">HL7 Form</xsl:with-param>
              <xsl:with-param name="SectionData" select="ActiveCompositionGroup/GenericItem/HL7Form/@name"/>
            </xsl:call-template>

          </tr>
        </xsl:if>
        <tr>

          <td class="monograph-heading ui-widget-content" style="font-weight: bold">Molecule</td>
          <td>
            <table>
              <tr class="Header">
                <td class="monograph-heading ui-widget-content" style="font-weight: bold">name</td>
                <td class="monograph-heading ui-widget-content" style="font-weight: bold">strength</td>
                <td class="monograph-heading ui-widget-content" style="font-weight: bold">unit</td>
                <xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolume) &gt;  0">
                  <td class="monograph-heading ui-widget-content" style="font-weight: bold">per Volume</td>
                </xsl:if>
                <xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolumeUnit) &gt;  0">
                  <td class="monograph-heading ui-widget-content" style="font-weight: bold">Unit</td>
                </xsl:if>
              </tr>
              <xsl:for-each select="ActiveCompositionGroup/ActiveComposition/Molecules/Molecule">
                <tr>
                  <td class="ui-widget-content" >
                    <xsl:value-of select="@name"/>
                  </td>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@strength"/>
                  </td>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@strengthUnit"/>
                  </td>
                  <xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolume) &gt;  0">
                    <td class="ui-widget-content">
                      <xsl:value-of select="@perVolume"/>
                    </td>
                  </xsl:if>
                  <xsl:if test="string-length(ActiveCompositionGroup/ActiveComposition/Molecules/Molecul/@perVolumeUnit) &gt;  0">
                    <td class="ui-widget-content">
                      <xsl:value-of select="@perVolumeUnit"/>
                    </td>
                  </xsl:if>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
        <xsl:if test="string-length(ActiveCompositionGroup/GenericItem/@name) &gt;  0">
          <tr>
            <td class="monograph-heading ui-widget-content" style="font-weight: bold">Generic Name</td>
            <td class="ui-widget-content">
              <xsl:value-of select="ActiveCompositionGroup/GenericItem/@name"/>
            </td>
          </tr>
        </xsl:if>
        <xsl:if test="Images/@count &gt;  0">
          <tr>
            <td class="monograph-heading ui-widget-content" style="font-weight: bold">Image</td>
            <td class="ui-widget-content">
              <img alt="PIC" src="C:\FTImage.gif"/>
            </td>
          </tr>
        </xsl:if>
      </xsl:for-each>
      <tr>
        <td class="monograph-heading ui-widget-content" style="font-weight: bold">Therapeutic Class</td>
        <td class="ui-widget-content">
          <ui>
            <xsl:for-each select="TherapeuticClasses/TherapeuticClass">
              <li >
                <xsl:value-of select="@name"/>
              </li>
            </xsl:for-each>
          </ui>
        </td>
      </tr>
      <xsl:if test="ATCCodes/@count &gt;  0">
        <tr>
          <td class="monograph-heading ui-widget-content" style="font-weight: bold">ATC Code</td>
          <td>
            <table width="100%">
              <tr>
                <td class="monograph-heading ui-widget-content" style="font-weight: bold">name</td>
                <td class="monograph-heading ui-widget-content" style="font-weight: bold">code</td>
              </tr>
              <xsl:for-each select="ATCCodes/ATCCode">
                <tr>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@name"/>
                  </td>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@code"/>
                  </td>
                </tr>
              </xsl:for-each>
              <xsl:for-each select="ATCCodes/AtcClassification">
                <tr>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@name"/>
                  </td>
                  <td class="ui-widget-content">
                    <xsl:value-of select="@code"/>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <td class="monograph-heading ui-widget-content" style="font-weight: bold">Company</td>
        <td>
          <table width="100%">
            <tr>
              <td  class="monograph-heading ui-widget-content" style="font-weight: bold" width="70%">name</td>
              <td  class="monograph-heading ui-widget-content" style="font-weight: bold">Type</td>
            </tr>
            <xsl:for-each select="Companies/Company">
              <tr>
                <td class="ui-widget-content">
                  <xsl:value-of select="@name"/>
                </td>
                <td class="ui-widget-content">
                  <xsl:value-of select="CompanyType/@name"/>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>
    <!--</xsl:if>-->
  </xsl:template>


  <xsl:template match="PackageType[not(preceding::PackageType=.)]">
    <h4 class="subheading">
      <xsl:value-of select="//Package/Product/@name"/>
    </h4>

    <table class="ui-widget monograph">

      <tr>
        <td class="monograph-heading ui-widget-content" style="font-weight: bold">Package</td>
        <td>
          <table style="width: 100%; border: 1px solid black;">
            <tr>
              <td class="ui-widget-content" style="width: 20%;font-weight: bold">Package Type</td>
              <td class="ui-widget-content" style="width: 20%;font-weight: bold">Items Per Pack</td>
              <td class="ui-widget-content" style="width: 20%;font-weight: bold">Quantity</td>
              <td class="ui-widget-content" style="width: 20%;font-weight: bold">Quantity Unit</td>
              <td class="ui-widget-content" style="width: 20%;font-weight: bold">Price</td>
            </tr>

            <xsl:for-each select="//Package/Product">

              <tr>
                <td class="ui-widget-content">
                  <xsl:value-of select="../PackageType/@name"/>
                </td>
                <td class="ui-widget-content">
                  <xsl:value-of select="../Items/Item/@itemsPerPack"/>
                </td>
                <td class="ui-widget-content">
                  <xsl:value-of select="../Items/Item/@quantity"/>
                </td>
                <td class="ui-widget-content">
                  <xsl:value-of select="../Items/Item/@quantityUnitCode"/>
                </td>
                <td class="ui-widget-content">
                  <xsl:value-of select="../Prices/Price/@price"/>
                </td>
              </tr>
            </xsl:for-each>

          </table>
        </td>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="VIDAL">
    <h4 class="subheading">
      <xsl:value-of select="../@name"/>
    </h4>
    <table class="ui-widget monograph">
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thành phần</xsl:with-param>
        <xsl:with-param name="SectionData" select="VC"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Sự miêu tả</xsl:with-param>
        <xsl:with-param name="SectionData" select="VDESC"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chỉ định/Công dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="VI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Liều lượng và cách dùng</xsl:with-param>
        <xsl:with-param name="SectionData" select="VD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Quá Liều</xsl:with-param>
        <xsl:with-param name="SectionData" select="VOD"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Chống chỉ định</xsl:with-param>
        <xsl:with-param name="SectionData" select="VCI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Cảnh báo</xsl:with-param>
        <xsl:with-param name="SectionData" select="VW"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Biện pháp phòng ngừa đặc biệt</xsl:with-param>
        <xsl:with-param name="SectionData" select="VSP"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Phản ứng có hại của thuốc</xsl:with-param>
        <xsl:with-param name="SectionData" select="VAR"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tác dụng ngoại ý</xsl:with-param>
        <xsl:with-param name="SectionData" select="VSE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Tương tác</xsl:with-param>
        <xsl:with-param name="SectionData" select="VDI"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Dược lực học/Dược động học</xsl:with-param>
        <xsl:with-param name="SectionData" select="VA"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Bảo quản</xsl:with-param>
        <xsl:with-param name="SectionData" select="VSTO"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Thận trọng khi sử dụng</xsl:with-param>
        <xsl:with-param name="SectionData" select="VCAU"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Trình bày/Đóng gói</xsl:with-param>
        <xsl:with-param name="SectionData" select="VPP"/>
      </xsl:call-template>
    </table>
  </xsl:template>
  <xsl:template match="SPECIFICPIL">
    <h4 class="subheading">
      <xsl:value-of select="../@name"/>
    </h4>
    <table class="ui-widget monograph">
      <!--	<xsl:call-template name="ShowSection"><xsl:with-param name="SectionName">Brand Name</xsl:with-param><xsl:with-param name="SectionData" select="Product"/></xsl:call-template><xsl:call-template name="ShowSection"><xsl:with-param name="SectionName">Drug Name</xsl:with-param><xsl:with-param name="SectionData" select="GGPI"/></xsl:call-template><xsl:call-template name="ShowSection"><xsl:with-param name="SectionName">Drug Name</xsl:with-param><xsl:with-param name="SectionData" select="GenericItem"/></xsl:call-template><xsl:call-template name="ShowSection"><xsl:with-param name="SectionName">Route</xsl:with-param><xsl:with-param name="SectionData" select="ROUTEFORM"/></xsl:call-template>  -->
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Patient Medicine Information</xsl:with-param>
        <xsl:with-param name="SectionData" select="DESCRIPTION"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Also Known as</xsl:with-param>
        <xsl:with-param name="SectionData" select="RELATEDSYNONYM"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="INDICATIONLABEL"/>
        <xsl:with-param name="SectionData" select="INDICATION"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="DOSAGELABEL"/>
        <xsl:with-param name="SectionData" select="DOSAGE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="MISSEDDOSELABEL"/>
        <xsl:with-param name="SectionData" select="MISSEDDOSE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="CONTRAINDICATIONSLABEL"/>
        <xsl:with-param name="SectionData" select="CONTRAINDICATIONS"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="SPECIALPRECAUTIONSLABEL"/>
        <xsl:with-param name="SectionData" select="SPECIALPRECAUTIONS"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="SIDEEFFECTSLABEL"/>
        <xsl:with-param name="SectionData" select="SIDEEFFECTS"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="DRUGINTERACTIONSLABEL"/>
        <xsl:with-param name="SectionData" select="DRUGINTERACTIONS"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="FOODINTERACTIONSLABEL"/>
        <xsl:with-param name="SectionData" select="FOODINTERACTIONS"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName" select="STORAGELABEL"/>
        <xsl:with-param name="SectionData" select="STORAGE"/>
      </xsl:call-template>
      <xsl:call-template name="ShowSection">
        <xsl:with-param name="SectionName">Copyright</xsl:with-param>
        <xsl:with-param name="SectionData" select="COPYRIGHT"/>
      </xsl:call-template>
    </table>
  </xsl:template>
  <xsl:template name="ShowSection">
    <xsl:param name="SectionName"/>
    <xsl:param name="SectionData"/>
    <xsl:if test="normalize-space($SectionData)">
      <tr>
        <td class="monograph-heading ui-widget-content">
          <span style="font-weight: bold">
            <xsl:value-of select="$SectionName"/>
          </span>
        </td>
        <td class="ui-widget-content">
          <xsl:value-of select="$SectionData" disable-output-escaping="yes"/>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>
  <xsl:template match="DrugItem|Product|GenericItem|SpecificItem|GGPI|ActiveComposition|ActiveCompositionGroup" mode="label">
    <xsl:variable name="IntType1" select="name()"/>
    <xsl:variable name="IntProd1" select="@name"/>
    <xsl:variable name="IntRef1" select="@reference"/>
    <div>
      <h3>
        <a href="#">
          <xsl:value-of select="$IntProd1"/>
        </a>
      </h3>
      <div>
        <div class="drug-label">
          <h4>
            <xsl:value-of select="$IntProd1"/>
          </h4>
          <xsl:apply-templates select=".//CautionaryLabel"/>
        </div>
        <xsl:apply-templates select=".//CautionaryLabel" mode="cals-details"/>
      </div>
    </div>
  </xsl:template>
  <xsl:template match="CautionaryLabel">
    <p>
      <xsl:value-of select="Warning"/>
    </p>
  </xsl:template>
  <xsl:template match="CautionaryLabel" mode="cals-details">
    <div class="cal-details">
      <p>
        <strong>
          <xsl:value-of select="@type"/> Warning:
        </strong>
        <xsl:value-of select="Warning"/>
      </p>
      <xsl:if test="Description">
        <p>
          <strong>Explanation:</strong>
          <xsl:value-of select="Description"/>
        </p>
      </xsl:if>
      <xsl:apply-templates select="References" mode="cals"/>
      <hr/>
    </div>
  </xsl:template>
  <xsl:template match="References" mode="cals">
    <h4 class="subheading">Tài liệu tham khảo</h4>
    <ul>
      <xsl:apply-templates select="Journal" mode="cals"/>
      <xsl:apply-templates select="Website" mode="cals"/>
    </ul>
  </xsl:template>
  <xsl:template match="Website|Journal" mode="cals">
    <li>
      <xsl:value-of select="Title"/>
      <xsl:text>. </xsl:text>
      <xsl:choose>
        <xsl:when test="URL">
          <xsl:element name="a">
            <xsl:attribute name="href">
              <xsl:value-of select="URL"/>
            </xsl:attribute>
            <i>
              <xsl:value-of select="ReferenceDisplay"/>
            </i>
            <xsl:text>. </xsl:text>
          </xsl:element>
        </xsl:when>
        <xsl:otherwise>
          <i>
            <xsl:value-of select="ReferenceDisplay"/>
          </i>
          <xsl:text>. </xsl:text>
        </xsl:otherwise>
      </xsl:choose>
      <!--<xsl:value-of select="URL"/>-->
    </li>
  </xsl:template>
</xsl:stylesheet>