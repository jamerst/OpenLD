import React, { Component } from "react";
import { Col, Row,
  Card,
  Button, Input, Label,
  Nav, NavItem, NavLink,
  TabContent, TabPane } from 'reactstrap';

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
              <TabPane tabId={mIndex} key={mIndex}>
                <Label for="modeName" className="mt-3">Mode Name</Label>
                <Input
                  id = "modeName"
                  required = {this.props.enabled}
                  value = {this.props.modes[mIndex].name}
                  onChange = {this.handleNameChange}
                />

                <Label className="mt-3" for="channels">Channels</Label>
                <Input
                  id = "channels"
                  type="number"
                  min = "0"
                  required = {this.props.enabled}
                  value = {this.props.modes[mIndex].channels}
                  onChange = {this.handleChannelChange}
                />
              </TabPane>
            );
          })}
        </TabContent>
      </Card>
    );
  }

  handleNameChange = (event) => {
    this.props.setModeName(this.state.activeMode, event.target.value);
  }

  handleChannelChange = (event) => {
    this.props.setModeChannels(this.state.activeMode, event.target.value);
  }

  setActiveMode = (mode) => {
    this.setState({activeMode: mode});
  }
}