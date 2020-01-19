import React, { Component, Fragment } from 'react';
import { Card, CardImg, CardBody, CardTitle, Col, Container, Row } from 'reactstrap';

export class FixtureResults extends Component {
  render = () => {
    if (this.props.results.length > 0) {
      return (
        <Fragment>
          <Container style={{maxHeight: this.props.height, height: this.props.height, overflow: "auto"}}>
            <Row className="justify-content-around">
            {this.props.results.map(result =>
              <Col xs={this.props.xs} md={this.props.md} key={result.id}>
                <Card color="light" onClick={() => this.props.onCardClick(result.id)} className="mb-3" style={{cursor: "pointer"}}>
                  <div className="p-3" style={{ width: "100%", textAlign: "center", backgroundColor: "white" }}>
                    <CardImg src={ "/api/fixture/GetImage/" + result.id } style={{ maxHeight: this.props.cardImgSize, width: "auto", maxWidth: "100%" }}></CardImg>
                  </div>
                  <CardBody>
                    <CardTitle className="h4">{result.name}</CardTitle>
                      <dl>
                        <dd>{result.manufacturer}</dd>
                        <dd>{result.type.name}</dd>

                        <dt>Power</dt>
                        <dd>{result.power}W</dd>
                      </dl>
                  </CardBody>
                </Card>
              </Col>
            )}
            </Row>
          </Container>
          <p className="text-center mt-3 mb-0"><em>Fixture information is not verified and may contain inaccuracies. No responsibility taken for data.</em></p>
        </Fragment>
      );
    } else {
      return (
        <p className="text-center" style={{height: this.props.height}}><em>No results found</em></p>
      )
    }
  }
}