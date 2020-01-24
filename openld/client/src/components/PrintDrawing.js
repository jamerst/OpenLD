import React, { Component, Fragment } from "react";
import { Page, Document, View, Image, Text } from "@react-pdf/renderer"
import authService from "./api-authorization/AuthorizeService";

export class PrintDrawing extends Component {
  render = () => {
    return (
      <Document>
        {this.props.views.map(view => {
          let imageStyle = {};
          if (view.width > view.height) {
            imageStyle["width"] = "100%";
          } else {
            imageStyle["height"] = "100%";
          }

          return (
            <Page
              size="A1"
              orientation="landscape"
              style = {{flexDirection: "row"}}
            >
              <View style={{border: "2mm solid #000", width: "80%", margin: "1cm", justifyContent: "center"}}>
                <Image
                  src={view.image}
                  style={{...imageStyle, ...{objectFit: "contain", alignSelf: "center"}}}
                />
              </View>
              <View style={{width: "20%"}}>
                <Text>{view.name}</Text>
                {view.usedFixtures.map(used => {
                  const image = new window.Image();
                  image.src = "/api/fixture/GetSymbol/" + used.fixture.id;
                  return (
                    <Fragment>
                      <Image
                        src={image}
                        style={{width: "1cm", height: "1cm"}}
                      />
                      <Text>{used.fixture.manufacturer} {used.fixture.name} {used.count}</Text>

                    </Fragment>
                  )
                })}
              </View>
            </Page>
          )
        })}
      </Document>
    )
  }
}