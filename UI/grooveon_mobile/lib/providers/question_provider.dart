

import 'package:grooveon_mobile/models/question_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';

class QuestionProvider extends BaseProvider<QuestionResponse> {
  QuestionProvider() : super("Question");

  @override
  QuestionResponse fromJson(data) {
    return QuestionResponse.fromJson(data);
  }
}