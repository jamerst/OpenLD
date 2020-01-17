import React, { Component, Fragment } from "react";
import { Alert, Collapse,
  Col, Container, Row,
  Card, CardHeader, CardBody,
  Button, CustomInput, Form, FormGroup, Input, Label,
  InputGroup, InputGroupAddon, InputGroupText,
  Modal, ModalHeader, ModalBody, ModalFooter,
  Nav, NavItem, NavLink, TabContent, TabPane,
  Spinner } from 'reactstrap';

export class FixtureModesForm extends Component {
  constructor(props) {
    super(props);

    this.state = {
      activeMode: 0
    }
  }


  render = () => {
    return (
      <Card className="p-2">
        <Row className="justify-content-end">
          <Col xs="auto">
            <Button color="secondary" size="sm" onClick={this.props.addMode}>Add Mode</Button>
          </Col>
        </Row>

        <Nav tabs>
          {this.props.modes.map((mode, index) => {
            return (
              <NavItem>
                <NavLink
                  className = {this.state.activeMode === index ? "active" : ""}
                  onClick = {() => this.setActiveMode(index)}
                >
                  {mode.name}
                </NavLink>
              </NavItem>
            );
          })}
        </Nav>

        <TabContent activeTab={this.state.activeMode}>
          {this.props.modes.map((mode, mIndex) => {
            return (
              <TabPane tabId={mIndex}>
                <Label for="modeName" className="mt-3">Mode Name</Label>
                <Input
                  id = "modeName"
                  required = {this.props.enabled}
                  value = {this.props.modes[mIndex].name}
                  onChange = {this.handleNameChange}
                />

                <Label className="mt-3">Channels</Label>
                <div className="vertical-input-group overflow-auto" style={{maxHeight: "15em"}}>
                  {mode.channels.map((channel, cIndex) => {
                    return (
                      <InputGroup>
                        <InputGroupAddon addonType="prepend">
                          <InputGroupText style={{width: "3.5em"}}>
                            <div className="text-right w-100">
                              {cIndex + 1}
                            </div>
                          </InputGroupText>
                        </InputGroupAddon>
                        <Input
                          required = {this.props.enabled}
                          value = {this.props.modes[mIndex].channels[cIndex]}
                          placeholder = "Name"
                          channel = {cIndex}
                          onChange = {this.handleChannelChange}
                        />
                      </InputGroup>
                    );
                  })}
                </div>
              </TabPane>
            );
          })}
        </TabContent>

        <Row>
          <Col>
            <Button color="secondary" className="mt-1" size="sm" onClick={() => this.props.addChannel(this.state.activeMode)}>Add Channel</Button>
          </Col>
        </Row>
      </Card>
    );
  }

  handleNameChange = (event) => {
    this.props.setModeName(this.state.activeMode, event.target.value);
  }

  handleChannelChange = (event) => {
    this.props.setChannelName(this.state.activeMode, event.target.getAttribute("channel"), event.target.value);
  }

  setActiveMode = (mode) => {
    this.setState({activeMode: mode});
  }
}