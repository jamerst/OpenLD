import React, { Component, Fragment } from "react";
import { Link } from 'react-router-dom';
import { PDFViewer } from "@react-pdf/renderer";
import { PrintDrawing } from "./PrintDrawing";
import { Stage, Layer } from "react-konva";
import { Container, Col, Row, Spinner } from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import authService from "./api-authorization/AuthorizeService";
import { View } from "./drawing/View";
import { Scale } from "./drawing/Scale";

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
      symbolsLoaded: false,
      rendered: false,
      viewer: null,
      loadingStatus: "Fetching Data"
    }
    this.loadedSymbols = {};
    this.images = {};
    this.views = [];
  }

  componentDidMount = () => {
    this.fetchDrawing();
  }

  render = () => {
    let loadingContent = null;
    const spinner = (
      <Container className="h-100">
        <Col className="h-100">
          <Row className="align-items-center justify-content-center h-100 flex-column">
            <Spinner style={{width: "10rem", height: "10rem"}}/>
            <h3 className="mt-3">{this.state.loadingStatus}</h3>
          </Row>
        </Col>
      </Container>
    );

    if (this.state.error === true) {
      return (
        <Container className="h-100">
          <Col className="h-100">
            <Row className="align-items-center justify-content-center flex-column h-100">
              {this.state.errorIcon}
              <h1 className="mt-3">{this.state.errorTitle}</h1>
              <h3>{this.state.errorMsg}</h3>
              <Link to="/">Return Home</Link>
            </Row>
          </Col>
        </Container>
        );
    } else if (this.state.dataLoaded === false) {
      return spinner;
    } else if (this.state.symbolsLoaded === false || this.state.rendered === false) {
      loadingContent = spinner;
    }

    if (this.state.dataLoaded === true && this.state.symbolsLoaded === true && this.state.rendered === false) {
      this.state.viewer = (
        <PDFViewer style={{width: "100%", height: "100%"}}>
          <PrintDrawing views={this.views} drawing={this.state.drawingData} onRender={() => {
            this.setState({rendered: true});
          }}/>
        </PDFViewer>
      );
    }

    return (
      <Fragment>
        {loadingContent}
        {this.state.drawingData.drawing.views.map(view => {
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
            <Layer>

              <Scale
                x = {1}
                y = {1}
                scale = {scale.scale}
              />
            </Layer>
              <View
                data = {view}
                onSymbolLoad = {this.onSymbolLoad}
              />
            </Stage>
          )
        })}
        {this.state.viewer}
      </Fragment>
    )
  }

  fetchDrawing = async () => {
    const response = await fetch("api/drawing/GetPrintDrawing/" + this.props.match.params.id, {
      headers: await authService.generateHeader()
    });

    if (response.ok) {
      const data = await response.json();

      data.data.drawing.views.forEach(view => {
        this.views.push(view);
      });

      this.setState({
        drawingData: data.data,
        dataLoaded: true,
        loadingStatus: "Loading Fixture Symbols"
      }, () => {
        this.sizeStage(this.scaleStage);
      });
    } else if (response.status === 401) {
      this.setState({
        error: true,
        errorTitle: "Access Denied",
        errorMsg: "Sorry, you don't have permission to view this drawing.",
        errorIcon: <FontAwesomeIcon icon="ban" style={{width: "15rem", height: "15rem", color: "#B71C1C"}}/>
      });
    } else if (response.status === 404) {
      this.setState({
        error: true,
        errorTitle: "Not Found",
        errorMsg: "The drawing you requested couldn't be found.",
        errorIcon: <FontAwesomeIcon icon={["far", "question-circle"]} style={{width: "15rem", height: "15rem", color: "#1565C0"}}/>
      });
    }
  }

  onSymbolLoad = (id) => {
    this.loadedSymbols[id] = true;
    this.setViewImage(id, this.stageRefs[id].current.toDataURL({pixelRatio: 5}), () => {
      let finished = true;
      Object.keys(this.loadedSymbols).forEach(key => {
        if (this.loadedSymbols[key] === false) {
          finished = false;
        }
      });

      if (finished === true) {
        this.setState({
          symbolsLoaded: true,
          loadingStatus: "Rendering Document"
        });
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