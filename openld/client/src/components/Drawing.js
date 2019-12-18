import React, { Component, Fragment } from 'react';
import { Link } from 'react-router-dom';
import {
  Button,
  Container, Row, Col,
  Navbar, NavbarBrand, NavLink } from 'reactstrap';
import { Layer, Line, Stage } from 'react-konva';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class Drawing extends Component {
  constructor(props) {
    super(props);
    this.state = {
      grid: [],
      stageWidth: 0,
      stageHeight: 0
    }

    this.renderGrid = this.renderGrid.bind(this);
    this.sizeStage = this.sizeStage.bind(this);
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

  renderGrid() {
    let grid = [];
    for (let x = 0; x < this.state.stageWidth; x+=50) {
      grid.push([x, 0, x, this.state.stageHeight]);
    }

    for (let y = 0; y < this.state.stageHeight; y+=50) {
      grid.push([0, y, this.state.stageWidth, y]);
    }
    this.setState({grid: grid});
  }
}