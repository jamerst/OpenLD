import React, { Component } from "react";
import { Col,
  Button, CustomInput,
  Card, CardHeader, CardBody,
  ListGroup, ListGroupItem, Label,
  Tooltip
} from "reactstrap";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { ShareDrawing } from "./ShareDrawing";
import { CreateViewForm } from "./CreateViewForm";
import { DeleteViewModal } from "./DeleteViewModal";

export class Sidebar extends Component {
  constructor(props) {
    super(props);

    this.state = {
      shareModalOpen: false,
      createViewOpen: false,
      deleteViewOpen: false,
      deletedViewId: "",
      deletedViewName: "test",
      gridTooltipOpen: false
    };
  }

  render = () => {
    return (
      <Col xs={this.props.xs} md={this.props.md} lg={this.props.lg} className="p-0 d-flex flex-column align-items-stretch bg-light" style={{maxHeight: this.props.height}}>
        <Card className="rounded-0" style={{minHeight: "15%"}}>
          <CardHeader className="d-flex justify-content-between align-content-center pl-3 pr-3">
            <h5 className="mb-0">Views</h5>
            <Button onClick={this.toggleCreateView} close><FontAwesomeIcon icon="plus-circle"/></Button>
            <CreateViewForm
              isOpen = {this.state.createViewOpen}
              toggle = {this.toggleCreateView}
              hub = {this.props.hub}
              drawingId = {this.props.drawingId}
            />
            <DeleteViewModal
              isOpen = {this.state.deleteViewOpen}
              toggle = {this.toggleDeleteView}
              hub = {this.props.hub}
              viewId = {this.state.deletedViewId}
              viewName = {this.state.deletedViewName}
            />
          </CardHeader>

          <CardBody className="overflow-y-auto p-0">
            <ListGroup>
              {this.props.views.map(view => {
                let button;
                if (this.props.views.length > 1) {
                  button = (
                  <Button close>
                    <FontAwesomeIcon
                      icon="trash"
                      size="xs"
                      className={this.props.currentView === view.id ? "text-white" : ""}
                      onClick={event => {
                        event.stopPropagation();
                        this.handleClickDelete(view.id, view.name);
                      }}
                    />
                  </Button>);
                }

                return (
                  <ListGroupItem
                    key = {"list-" + view.id}
                    className="rounded-0 p-1 border-left-0 border-right-0 d-flex justify-content-between text-break"
                    onClick={() => this.props.onClickView(view.id)}
                    active={this.props.currentView === view.id}
                    style={{cursor: "pointer"}}
                  >
                    <div>{view.name}</div>
                    {button}
                  </ListGroupItem>
                );
              })}
            </ListGroup>
          </CardBody>
        </Card>

        <Card className="rounded-0" style={{minHeight: "15%"}}>
          <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Selected Object</h5></CardHeader>
          <CardBody className="overflow-auto text-center">
            <em>No object selected</em>
          </CardBody>
        </Card>

        <Card className="rounded-0" style={{minHeight: "15%"}}>
          <CardHeader className="pl-3 pr-3"><h5 className="mb-0">Drawing Properties</h5></CardHeader>
          <CardBody className="overflow-auto">
            <Button color="primary" size="sm" onClick={this.toggleShareModal}>Sharing Settings</Button>
            <CustomInput type="switch" onChange={this.props.toggleGrid} checked={this.props.gridEnabled} id="grid-toggle" label="Show Grid"/>

            <Label for="grid-size">Grid Size</Label>
            <CustomInput
              type="range" min="1" max="20" step="1" name="gridSize" id="grid-size"
              value={this.props.gridSize}
              onChange={event => this.props.setGridSize(parseFloat(event.target.value))}
            />
            <Tooltip isOpen={this.state.gridTooltipOpen} target="grid-size" toggle={this.toggleGridTooltip} hideArrow>
              {this.props.gridSize}m
            </Tooltip>
          </CardBody>
        </Card>
        <ShareDrawing
          isOpen = {this.state.shareModalOpen}
          drawingId = {this.props.drawingId}
          toggle = {this.toggleShareModal}
        />
      </Col>
    );
  }

  toggleShareModal = () => {
    this.setState({shareModalOpen: !this.state.shareModalOpen});
  }

  toggleCreateView = () => {
    this.setState({createViewOpen: !this.state.createViewOpen});
  }

  toggleDeleteView = () => {
    this.setState({deleteViewOpen: !this.state.deleteViewOpen});
  }

  handleClickDelete = (id, name) => {
    this.setState({
      deletedViewId: id,
      deletedViewName: name,
      deleteViewOpen: true
    })
  }

  toggleGridTooltip = () => {
    this.setState({gridTooltipOpen: !this.state.gridTooltipOpen});
  }
}