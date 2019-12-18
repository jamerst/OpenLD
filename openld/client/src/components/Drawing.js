import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import {
  Button,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink } from 'reactstrap';
import { Layer, Line, Stage, Text } from 'react-konva';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class Drawing extends Component {
  constructor(props) {
    super(props);
    this.state = {
      grid: [],
      gridSize: 50,
      stageWidth: 0,
      stageHeight: 0
    }

    this.renderGrid = this.renderGrid.bind(this);
    this.sizeStage = this.sizeStage.bind(this);
    this.handleDrag = this.handleDrag.bind(this);
  }

  componentDidMount() {
    this.sizeStage();

    window.addEventListener("resize", this.sizeStage);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.sizeStage);
  }

  render() {
    return (
      <div className="d-flex flex-column h-100">
        <Navbar color="light">
          <NavbarBrand>[drawing title]</NavbarBrand>
          <NavLink tag={Link} to="/"><Button close/></NavLink>
        </Navbar>
        <Container fluid className="pl-0 d-flex flex-grow-1">
          <Row className="d-flex flex-grow-1">
            <Col xs="auto" className="d-flex flex-column pr-0">
              <Button className="mb-1" outline size="lg"><FontAwesomeIcon icon="draw-polygon" /></Button>
              <Button className="mb-1" outline size="lg"><FontAwesomeIcon icon="eraser" /></Button>
            </Col>
            <Col id="stage-container" className="p-0 m-0 flex-grow-1">
              <Stage
                width={this.state.stageWidth}
                height={this.state.stageHeight}
              >
                <Layer ref="grid">
                  {this.state.grid.map((line, index) => {
                    return (
                      <Line
                        key = {index}
                        points = {line}
                        stroke = "#ddd"
                        strokeWidth = {0.5}
                      />
                    );
                  })}
                </Layer>
                <Layer>
                  <Line
                    key = "test"
                    points = {[0, 0, 100, 100]}
                    stroke = "#000"
                    strokeWidth = {5}
                    draggable = {true}
                    onDragEnd = {this.handleDrag}
                  />
                </Layer>
                <Layer>
                  <Line
                    key = "scale"
                    points = {[
                      (Math.floor(this.state.stageWidth / 50) - 1) * 50, Math.floor(this.state.stageHeight / 50) * 50,
                      Math.floor(this.state.stageWidth / 50) * 50, Math.floor(this.state.stageHeight / 50) * 50
                    ]}
                    stroke = "#000"
                  />
                  <Text
                    key = "scaleLabel"
                    x = {(Math.floor(this.state.stageWidth / 50) - 1) * 50}
                    y = {(Math.floor(this.state.stageHeight / 50) - 0.25) * 50}
                    text = "1m"
                  />

                </Layer>
              </Stage>
            </Col>
            <Col xs="1" style={{backgroundColor: "green"}}>
            hello
            </Col>
          </Row>
        </Container>
      </div>
    );
  }

  sizeStage() {
    const container = document.getElementById("stage-container");
    this.setState({
      stageWidth: container.clientWidth,
      stageHeight: container.clientHeight
    }, () => {
      this.renderGrid();
    });

  }

  handleDrag(event) {
    const pos = event.target.position();
    event.target.position({x: Math.round(pos.x / this.state.gridSize) * this.state.gridSize,
      y: Math.round(pos.y / this.state.gridSize) * this.state.gridSize});
    event.target.getLayer().draw();
  }

  renderGrid() {
    let grid = [];
    for (let x = 0; x < 2 * this.state.stageWidth; x+=this.state.gridSize) {
      grid.push([x, 0, x, 2 * this.state.stageHeight]);
    }

    for (let y = 0; y < 2 * this.state.stageHeight; y+=this.state.gridSize) {
      grid.push([0, y, 2 * this.state.stageWidth, y]);
    }
    this.setState({grid: grid});
  }
}