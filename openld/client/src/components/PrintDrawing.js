import React, { Component } from "react";
import { Page, Document, View, Image, StyleSheet, Text } from "@react-pdf/renderer"

const styles = StyleSheet.create({
  drawingContainer: {border: "1mm solid #000", width: "80%", margin: "1cm", justifyContent: "center"},
  sideContainer: {width: "20%", margin: "1cm", marginLeft: "0", justifyContent: "space-between"},
  keyContainer: {width: "82.5%", border: "1mm solid #000", padding: "5mm", paddingBottom: "0"},
  keyEntry: {flexDirection: "row", justifyContent: "space-between", alignItems: "center", marginBottom: "5mm"},
  keyText: {fontSize: "8mm"},
  detailContainer: {width: "82.5%", border: "1mm solid #000", padding: "5mm", paddingBottom: "1mm"},
  detailEntry: {marginBottom: "2mm"},
  credit: {fontSize: "5mm", color: "#ccc", textAlign: "center"},
  tableHeader: {flexDirection: "row", borderBottom: "2 solid #000", marginTop: "5mm"},
  headCell10: {width: "10%", fontSize: "5mm", fontFamily: "Helvetica-Bold"},
  headCell20: {width: "20%", fontSize: "5mm", fontFamily: "Helvetica-Bold"},
  headCell30: {width: "30%", fontSize: "5mm", fontFamily: "Helvetica-Bold"},
  tableRow: {flexDirection: "row", borderBottom: "1 solid #000", paddingVertical: "3mm", paddingHorizontal: "2mm"},
  cell10: {width: "10%", fontSize: "4mm", display: "flex", justifyContent: "center"},
  cell20: {width: "20%", fontSize: "4mm", display: "flex", justifyContent: "center"},
  cell30: {width: "30%", fontSize: "4mm", display: "flex", justifyContent: "center"},
  bold: {fontFamily: "Helvetica-Bold"},
  italic: {fontFamily: "Helvetica-Oblique"}
});

export class PrintDrawing extends Component {

  render = () => {
    return (
      <Document
        title = {this.props.drawing.drawing.name}
        author = {this.props.drawing.drawing.owner.userName}
        creator = {"OpenLD"}
        onRender = {this.props.onRender}
      >
        {this.props.views.map((view, index) => {
          let imageStyle = {};
          if (view.width > view.height) {
            imageStyle["width"] = "100%";
          } else {
            imageStyle["height"] = "100%";
          }

          return (
            <Page
              key = {`page-${view.id}`}
              size="A1"
              orientation="landscape"
              style = {{flexDirection: "row"}}
            >
              <View style={styles.drawingContainer}>
                <Image
                  src={view.image}
                  style={{...imageStyle, ...{objectFit: "contain", alignSelf: "center"}}}
                />
              </View>
              <View style={styles.sideContainer}>
                <View style={styles.keyContainer}>
                  {this.props.drawing.usedFixtures[index].map(used => {
                    return (
                      <View style={styles.keyEntry} key={`uf-${used.fixture.id}`}>
                        <Image
                          src={`/api/fixture/GetSymbolBitmap/${used.fixture.id}`}
                          style={{objectFit: "contain", width: "1.5cm", height: "1.5cm"}}
                        />
                        <Text style={[styles.keyText, {maxWidth: "9cm"}]}>{used.fixture.manufacturer} {used.fixture.name}</Text>
                        <Text style={styles.keyText}>{used.count}</Text>
                      </View>
                    )
                  })}
                </View>
                <View style={styles.detailContainer}>
                    <Text>
                      <Text style={styles.bold}>View Name: </Text>
                      {view.name}
                    </Text>
                    <Text>
                      <Text style={styles.bold}>Type: </Text>
                      {view.type === 0 ? "Top-down" : "Front-on"}
                    </Text>
                    <Text>
                      <Text style={styles.bold}>Drawing Title: </Text>
                      {this.props.drawing.drawing.title}
                    </Text>
                    <Text style={{marginBottom: "5mm"}}>
                      <Text style={styles.bold}>Created By: </Text>
                      {this.props.drawing.drawing.owner.userName}
                    </Text>

                    <Text style={{fontSize: "7mm", textAlign: "center", marginBottom: "5mm"}}>
                      View <Text style={styles.italic}>{index+1}</Text> of <Text style={styles.italic}>{this.props.views.length}</Text>
                    </Text>

                    <Text style={[styles.italic, {textAlign: "center"}]}>
                      Generated {new Date().toLocaleString()}
                    </Text>

                  <Text style={styles.credit}>Created with OpenLD &bull; www.openld.jtattersall.net</Text>
                </View>
              </View>
            </Page>
          )
        })}

        <Page size="A4" orientation="landscape">
          <View style={{margin: "15mm"}}>
            <Text style={styles.bold}>Channel List - {this.props.drawing.drawing.title}</Text>
            <Text style={[styles.italic, {fontSize: "4mm", color: "#aaa"}]}>
              Generated {new Date().toLocaleString()}
            </Text>

            <View style={styles.tableHeader}>
              <Text style={styles.headCell10}>Channel</Text>
              <Text style={styles.headCell20}>Name</Text>
              <Text style={styles.headCell10}>Address</Text>
              <Text style={styles.headCell20}>Mode</Text>
              <Text style={styles.headCell10}>Colour</Text>
              <Text style={styles.headCell30}>Notes</Text>
            </View>
            {this.props.drawing.riggedFixtures.map(fixture => {
              return (
                <View style={styles.tableRow} key={`fr-${fixture.id}`}>
                  <View style={styles.cell10}><Text>{fixture.channel}</Text></View>
                  <View style={styles.cell20}>
                    <Text>
                      {
                        fixture.name
                          ? `${fixture.name} (${fixture.fixture.manufacturer} ${fixture.fixture.name})`
                          : `${fixture.fixture.manufacturer} ${fixture.fixture.name}`
                      }
                    </Text>
                  </View>
                  <View style={styles.cell10}><Text>{fixture.universe}-{fixture.address}</Text></View>
                  <View style={styles.cell20}><Text>{`${fixture.mode.name} (${fixture.mode.channels} channel)`}</Text></View>
                  <View style={styles.cell10}><Text>{fixture.colour}</Text></View>
                  <View style={styles.cell30}><Text>{fixture.notes}</Text></View>
                </View>
              )
            })}
          </View>
        </Page>
      </Document>
    )
  }
}