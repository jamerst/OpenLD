import React, { Component, Fragment } from "react";
import { PDFViewer } from "@react-pdf/renderer";
import { PrintDrawing } from "./PrintDrawing";
import { Stage } from "react-konva";
import authService from "./api-authorization/AuthorizeService";
import { View } from "./drawing/View";

export class PrintDrawingViewer extends Component {
  constructor(props) {
    super(props);
    this.stageRefs = {};
    this.state = {
      drawingData: null,
      stageScale: 50,
      stageWidth: 0,
      stageHeight: 0,
      stageX: 0,
      stageY: 0,
      stagePosition: {x: 0, y: 0},
      dataLoaded: false,
      symbolsLoaded: false
    }
    this.loadedSymbols = {};
    this.images = {};
    this.views = [];
  }

  componentDidMount = () => {
    this.fetchDrawing();
  }

  render = () => {
    if (this.state.dataLoaded === false) {
      return null;
    }

    let viewer;
    if (this.state.dataLoaded === true && this.state.symbolsLoaded === true) {
      viewer = (
        <PDFViewer style={{width: "100%", height: "100%"}}>
          <PrintDrawing views={this.views}/>
        </PDFViewer>
      );
    }

    return (
      <Fragment>
        {this.state.drawingData.printViews.map(view => {
          this.stageRefs[view.id] = React.createRef();
          this.loadedSymbols[view.id] = false;

          let size = this.sizeStage(view);
          let scale = this.scaleStage(view, size.width, size.height, size.y);
          return (
            <Stage
              key = {"stage-" + view.id}
              style={{display: "none"}}
              ref = {this.stageRefs[view.id]}
              width = {size.width}
              height = {size.height}
              scale = {{x: scale.scale, y: -scale.scale}}
              offsetY = {size.y}
              position = {scale.position}
              draggable
            >
              <View
                data = {view}
                onSymbolLoad = {this.onSymbolLoad}
              />
            </Stage>
          )
        })}
        {viewer}
      </Fragment>
    )
  }

  fetchDrawing = async () => {
    const response = await fetch("api/drawing/GetPrintDrawing/" + this.props.match.params.id, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();

      data.data.printViews.forEach(view => {
        this.views.push(view);
      });

      this.setState({
        drawingData: data.data,
        dataLoaded: true
      }, () => {
        this.sizeStage(this.scaleStage);
      });
    }
  }

  onSymbolLoad = (id) => {
    this.loadedSymbols[id] = true;
    this.setViewImage(id, this.stageRefs[id].current.toDataURL({pixelRatio: 2}), () => {
      let finished = true;
      Object.keys(this.loadedSymbols).forEach(key => {
        if (this.loadedSymbols[key] === false) {
          finished = false;
        }
      });

      if (finished === true) {
        this.setState({symbolsLoaded: true});
      }
    })
  }

  setViewImage = (id, image, callback) => {
    const viewIndex = this.views.findIndex(v => v.id === id);

    this.views[viewIndex].image = image;

    if (callback && typeof callback === "function") {
      callback();
    }
  }

  sizeStage = (view) => {
    let width, height;
    if (view.width >= view.height) {
      width = 1000;
      height = 1000 * view.height / view.width
    } else {
      height = 1000;
      width = 1000 * view.width / view.height
    }
    return {
      width: width,
      height: height,
      x: width / 50,
      y: height / 50,
    };

  }

  scaleStage = (view, width, height, y) => {
    // set the scale such that the entire view can be seen

    let scale = Math.min(
      width / view.width,
      height / view.height
    );

    // center the drawing
    return {
      scale: scale,
      position: {
        x: (width - view.width * scale) / 2,
        y: (height + view.height * scale) / 2 - y * scale
      }
    }
  }
}