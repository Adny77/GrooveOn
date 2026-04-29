

import 'package:grooveon_mobile/models/answer_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';

class AnswerProvider extends BaseProvider<AnswerResponse> {
  AnswerProvider() : super("Answer");

  @override
  AnswerResponse fromJson(data) {
    return AnswerResponse.fromJson(data);
  }
}